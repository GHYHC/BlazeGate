using AntDesign.ProLayout;
using BlazeGate.BlazorWasmApp.Sample.Api;
using BlazeGate.Model.Culture;
using BlazeGate.RBAC.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

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

            var host = builder.Build();

            await SetCultureAsync(host, LanguageOptions.Languages[0]);

            await host.RunAsync();
        }

        /// <summary>
        /// 设置应用程序的文化信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="defaultCulture"></param>
        /// <returns></returns>
        private static async Task SetCultureAsync(WebAssemblyHost host, string defaultCulture)
        {
            var js = host.Services.GetRequiredService<IJSRuntime>();
            var result = await js.InvokeAsync<string>("blazorCulture.get");
            var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

            if (result == null)
            {
                await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}