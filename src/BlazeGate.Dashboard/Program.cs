using AntDesign.ProLayout;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlazeGate.Common.Autofac;
using BlazeGate.Dashboard.Components;
using BlazeGate.Model.Culture;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntDesign();
builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
builder.Services.AddInteractiveStringLocalizer();
builder.Services.AddLocalization();

//添加认证
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

//添加数据库
builder.Services.AddDbContext<BlazeGateContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BlazeGate.Model")));

//添加Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApiResult<>).Assembly);

//添加Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    //批量自动注入,把需要注入层的程序集传参数
    containerBuilder.BatchAutowired(typeof(Program).Assembly, typeof(BlazeGate.Services.Implement.UserRoleService).Assembly);
});

//添加内存缓存
builder.Services.AddDistributedMemoryCache();

//添加HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

//自动迁移数据库
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize().Wait();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

// 使用以根据客户端提供的信息自动设置请求的文化信息
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture(LanguageOptions.Languages[0])
    .AddSupportedCultures(LanguageOptions.Languages)
    .AddSupportedUICultures(LanguageOptions.Languages));

app.UseStaticFiles();
app.UseAntiforgery();

app.MapDefaultControllerRoute();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();