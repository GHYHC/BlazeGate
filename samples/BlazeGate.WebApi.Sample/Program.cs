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

//添加数据库
builder.Services.AddDbContext<BlazeGateSampleContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(BlazeGateSampleContext).Assembly.GetName().Name)));

//添加JwtBearer认证
builder.AddBlazeGateJwtBearer();

//添加BlazeGate
builder.AddBlazeGate();

//添加健康检查
builder.Services.AddHealthChecks();

//添加雪花算法
builder.Services.AddBlazeGateSnowFlake();

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

var app = builder.Build();

//自动迁移数据库
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BlazeGateSampleContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

//使用转发头信息中间件（有反向代理获取真实IP）
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//使用健康检查
app.UseHealthChecks("/api/health");

// 添加身份验证中间件
app.UseAuthentication();

// 添加授权中间件
app.UseAuthorization();

app.MapControllers();

app.Run();