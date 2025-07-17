using AntDesign.ProLayout;
using BlazeGate.Components.Sample.Api;
using BlazeGate.RBAC.Components;
using BlazeGate.RBAC.Components.Extensions.AuthTokenStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazeGate.BlazorWebApp.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped(
                sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            AddClientServices(builder.Services);

            builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

            var host = builder.Build();
            await host.SetCultureAsync();
            await host.RunAsync();
        }

        public static void AddClientServices(IServiceCollection services)
        {
            services.AddAntDesign();
            services.AddBlazeGateRBAC(AuthTokenStorageEnum.LocalStorage);
            services.AddScoped<WebApi>();
        }
    }
}