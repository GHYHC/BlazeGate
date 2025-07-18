using AntDesign.ProLayout;
using BlazeGate.BlazorWebApp.Sample.Components;
using BlazeGate.Components.Sample.Api;
using BlazeGate.Model.Culture;
using BlazeGate.RBAC.Components;
using BlazeGate.RBAC.Components.Extensions.AuthTokenStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    if (httpContext != null)
    {
        return new HttpClient
        {
            BaseAddress = new Uri(httpContext.Request.Scheme + "://" + httpContext.Request.Host)
        };
    }
    return new HttpClient();
});

builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

builder.Services.AddAntDesign();
builder.Services.AddBlazeGateRBAC(AuthTokenStorageEnum.LocalStorage);
builder.Services.AddScoped<WebApi>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.SetCultureAsync();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazeGate.BlazorWebApp.Client.Sample._Imports).Assembly, typeof(BlazeGate.RBAC.Components._Imports).Assembly, typeof(BlazeGate.Components.Sample._Imports).Assembly)
    ;

app.Run();