using BlazeGate.Model.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazeGate.AspNetCore
{
    public static class BlazeGateServiceExtension
    {
        public static IHostApplicationBuilder AddBlazeGate(this IHostApplicationBuilder builder, Action<BlazeGateOptions> options = null)
        {
            //先从配置文件中读取配置，然后再读取传入的配置
            builder.Services.Configure<BlazeGateOptions>(builder.Configuration.GetSection("BlazeGate").Bind);

            if (options != null)
            {
                builder.Services.Configure(options);
            }

            builder.Services.AddHttpClient();
            builder.Services.AddHostedService<BlazeGateService>();

            return builder;
        }
    }
}