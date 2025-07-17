using AntDesign.ProLayout;
using BlazeGate.Components.Sample.Api;
using BlazeGate.RBAC.Components;
using BlazeGate.RBAC.Components.Extensions.AuthTokenStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazeGate.BlazorWebApp.Client.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped(
                sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

            builder.Services.AddAntDesign();
            builder.Services.AddBlazeGateRBAC(AuthTokenStorageEnum.LocalStorage);
            builder.Services.AddScoped<WebApi>();

            var host = builder.Build();
            await host.SetCultureAsync();
            await host.RunAsync();
        }
    }
}