using AntDesign.ProLayout;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlazeGate.Common.Autofac;
using BlazeGate.Dashboard.Components;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
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

//�����֤
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

//������ݿ�
builder.Services.AddDbContext<BlazeGateContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("BlazeGate.Model")));

//���Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApiResult<>).Assembly);

//���Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    //�����Զ�ע��,����Ҫע���ĳ��򼯴�����
    containerBuilder.BatchAutowired(typeof(Program).Assembly, typeof(BlazeGate.Services.Implement.UserRoleService).Assembly);
});

//����ڴ滺��
builder.Services.AddDistributedMemoryCache();

//���HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

//�Զ�Ǩ�����ݿ�
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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapDefaultControllerRoute();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();