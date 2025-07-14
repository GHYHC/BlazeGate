using AntDesign.ProLayout;
using BlazeGate.BlazorWasmApp.Sample.Api;
using BlazeGate.RBAC.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazeGate.BlazorWasmApp.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAntDesign();
            builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
            builder.Services.AddLocalization();

            builder.Services.AddBlazeGateRBAC();
            builder.Services.AddScoped<WebApi>();

            await builder.Build().RunAsync();
        }
    }
}