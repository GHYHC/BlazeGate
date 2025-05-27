using BlazeGate.AspNetCore;
using BlazeGate.JwtBearer;
using BlazeGate.Model.Sample.EFCore;
using BlazeGate.Services.Implement.Remote;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//������ݿ�
builder.Services.AddDbContext<BlazeGateSampleContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(BlazeGateSampleContext).Assembly.GetName().Name)));

//���JwtBearer��֤
builder.AddBlazeGateJwtBearer();

//���BlazeGate
builder.AddBlazeGate();

//��ӽ������
builder.Services.AddHealthChecks();

//���ѩ���㷨
builder.Services.AddBlazeGateSnowFlake();

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

var app = builder.Build();

//�Զ�Ǩ�����ݿ�
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BlazeGateSampleContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

//ʹ��ת��ͷ��Ϣ�м�����з�������ȡ��ʵIP��
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//ʹ�ý������
app.UseHealthChecks("/api/health");

// ��������֤�м��
app.UseAuthentication();

// �����Ȩ�м��
app.UseAuthorization();

app.MapControllers();

app.Run();