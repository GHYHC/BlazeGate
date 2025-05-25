using BlazeGate.BackgroundService;

namespace BlazeGate
{
    public static class ServiceCollectionExtensions
    {
        public static IReverseProxyBuilder LoadFromDatabase(this IReverseProxyBuilder builder)
        {
            builder.LoadFromMemory(default, default);
            builder.Services.AddHostedService<ProxyConfigUpdate>();

            return builder;
        }
    }
}
