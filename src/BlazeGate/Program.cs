using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlazeGate;
using BlazeGate.Authentication;
using BlazeGate.Authorization;
using BlazeGate.AuthWhiteList;
using BlazeGate.BackgroundService;
using BlazeGate.Common.Autofac;
using BlazeGate.JwtBearer;
using BlazeGate.Model.Culture;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Policy;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Yarp.ReverseProxy.Health;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//������ݿ�
builder.Services.AddDbContext<BlazeGateContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BlazeGate.Model")));

//���Yarp�������ݿ��������
builder.Services.AddReverseProxy().LoadFromDatabase();

// ��ӷֲ�ʽ����������ݿ⣩
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "dbo";
    options.TableName = "DistributedCache";
});

//���Token����
builder.AddAuthenticationTokenService();

//�����֤(JwtBearer)
builder.AddAuthentication(false);

//�����Ȩ
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RBAC", policy =>
    {
        policy.Requirements.Add(new RBACRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, RBACHandler>();
builder.Services.AddScoped<IRBACService, RBACService>();
// IAuthorizationMiddlewareResultHandler �����滻���Ĭ�ϵ���Ȩ���ؽ��
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

//��ӱ���������FirstUnsuccessfulResponse
builder.Services.AddSingleton<IPassiveHealthCheckPolicy, FirstUnsuccessfulResponseHealthPolicy>();

//�����Ӧ����
builder.Services.AddResponseCaching(options =>
{
    options.UseCaseSensitivePaths = false; //ȷ���Ƿ���Ӧ���������ִ�Сд��·���ϡ�
    options.SizeLimit = options.SizeLimit * 10; // ��Ӧ�����м���Ĵ�С���ƣ����ֽ�Ϊ��λ�� 1G
});

//�����Ӧѹ��
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

//����ڴ滺�����
builder.Services.AddMemoryCache();

//���Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApiResult<>).Assembly);

//���ÿ���
builder.Services.AddCors(options =>
{
    options.AddPolicy("any", builder =>
    {
        //�����κ���Դ����������
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("*");
    });
});

//���Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    //�����Զ�ע��,����Ҫע���ĳ��򼯴�����
    containerBuilder.BatchAutowired(typeof(Program).Assembly, typeof(BlazeGate.Services.Implement.UserRoleService).Assembly);
});

//��ӽ������
builder.Services.AddHealthChecks();

//���ѩ���㷨ID������
builder.Services.AddHostedService<SnowFlakeIdCheck>();

//���httpClient
builder.Services.AddHttpClient();

//����ת��ͷ��Ϣѡ��
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All;

    foreach (var ip in builder.Configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? [])
        if (System.Net.IPAddress.TryParse(ip, out var parsedIp))
            options.KnownProxies.Add(parsedIp);

    foreach (var network in builder.Configuration.GetSection("ForwardedHeaders:KnownNetworks").Get<string[]>() ?? [])
        if (Microsoft.AspNetCore.HttpOverrides.IPNetwork.TryParse(network, out var parsedNetwork))
            options.KnownNetworks.Add(parsedNetwork);
});

//��ӱ��ػ�֧��
builder.Services.AddLocalization();

var app = builder.Build();

//�Զ�Ǩ�����ݿ�
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize().Wait();
}

// ʹ���Ը��ݿͻ����ṩ����Ϣ�Զ�����������Ļ���Ϣ
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture(LanguageOptions.Languages[0])
    .AddSupportedCultures(LanguageOptions.Languages)
    .AddSupportedUICultures(LanguageOptions.Languages)
);

//ʹ��ת��ͷ��Ϣ�м�����з�������ȡ��ʵIP��
app.UseForwardedHeaders();

//ʹ����Ӧѹ��
app.UseResponseCompression();

//ʹ����Ӧ����
app.UseResponseCaching();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//��ӽ������
app.UseHealthChecks("/api/health");

app.UseCors("any");

//�����֤������
app.UseAuthWhiteList();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy();

app.Run();