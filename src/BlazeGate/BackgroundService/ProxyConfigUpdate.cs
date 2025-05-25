using BlazeGate.Services.Interface;

namespace BlazeGate.BackgroundService
{
    public class ProxyConfigUpdate : Microsoft.Extensions.Hosting.BackgroundService
    {
        private const int IntervalSeconds = 60;
        private readonly IServiceProvider serviceProvider;

        public ProxyConfigUpdate(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    IYarpConfigService yarpConfig = scope.ServiceProvider.GetRequiredService<IYarpConfigService>();
                    await yarpConfig.Update(null);
                }
                await Task.Delay(TimeSpan.FromSeconds(IntervalSeconds), stoppingToken);
            }
        }
    }
}