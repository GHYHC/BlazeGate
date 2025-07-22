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

//添加数据库
builder.Services.AddDbContext<BlazeGateContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BlazeGate.Model")));

//添加Yarp并从数据库加载配置
builder.Services.AddReverseProxy().LoadFromDatabase();

// 添加分布式缓存服务（数据库）
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "dbo";
    options.TableName = "DistributedCache";
});

//添加Token服务
builder.AddAuthenticationTokenService();

//添加认证(JwtBearer)
builder.AddAuthentication(false);

//添加授权
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RBAC", policy =>
    {
        policy.Requirements.Add(new RBACRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, RBACHandler>();
builder.Services.AddScoped<IRBACService, RBACService>();
// IAuthorizationMiddlewareResultHandler 用来替换框架默认的授权返回结果
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

//添加被动检查策略FirstUnsuccessfulResponse
builder.Services.AddSingleton<IPassiveHealthCheckPolicy, FirstUnsuccessfulResponseHealthPolicy>();

//添加响应缓存
builder.Services.AddResponseCaching(options =>
{
    options.UseCaseSensitivePaths = false; //确定是否将响应缓存在区分大小写的路径上。
    options.SizeLimit = options.SizeLimit * 10; // 响应缓存中间件的大小限制（以字节为单位） 1G
});

//添加响应压缩
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

//添加内存缓存服务
builder.Services.AddMemoryCache();

//添加Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApiResult<>).Assembly);

//配置跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("any", builder =>
    {
        //允许任何来源的主机访问
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("*");
    });
});

//添加Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    //批量自动注入,把需要注入层的程序集传参数
    containerBuilder.BatchAutowired(typeof(Program).Assembly, typeof(BlazeGate.Services.Implement.UserRoleService).Assembly);
});

//添加健康检查
builder.Services.AddHealthChecks();

//添加雪花算法ID检查服务
builder.Services.AddHostedService<SnowFlakeIdCheck>();

//添加httpClient
builder.Services.AddHttpClient();

//配置转发头信息选项
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

//添加本地化支持
builder.Services.AddLocalization();

var app = builder.Build();

//自动迁移数据库
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize().Wait();
}

// 使用以根据客户端提供的信息自动设置请求的文化信息
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture(LanguageOptions.Languages[0])
    .AddSupportedCultures(LanguageOptions.Languages)
    .AddSupportedUICultures(LanguageOptions.Languages)
);

//使用转发头信息中间件（有反向代理获取真实IP）
app.UseForwardedHeaders();

//使用响应压缩
app.UseResponseCompression();

//使用响应缓存
app.UseResponseCaching();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//添加健康检查
app.UseHealthChecks("/api/health");

app.UseCors("any");

//添加认证白名单
app.UseAuthWhiteList();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapReverseProxy();

app.Run();