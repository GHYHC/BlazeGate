using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazeGate.Services.Implement.Remote
{
    public class SnowFlakeService : BaseWebApi, ISnowFlakeService
    {
        private readonly ILogger<SnowFlakeService> logger;

        public SnowFlakeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            WebApiAddress = BlazeGateAddress;
            logger = serviceProvider.GetRequiredService<ILogger<SnowFlakeService>>();
        }

        public Task<SnowFlakeInfo> GetSnowFlakeInfo()
        {
            throw new NotImplementedException();
        }

        public async Task<long> NextId()
        {
            // 重试3次
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    return await HttpPostJsonAsync<string, long>("/api/SnowFlake/NextId", "");
                }
                catch (Exception ex)
                {
                    // 记录错误日志
                    logger.LogError(ex, "获取下一个SnowFlake ID失败，尝试次数：{Attempt}", attempt);

                    if (attempt == 3) throw;
                    await Task.Delay(1000);
                }
            }

            // 理论不可达
            throw new InvalidOperationException("Unexpected flow in NextId retry.");
        }

        public async Task<List<long>> NextIds(int count)
        {
            string result = string.Empty;

            // 重试3次
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    result = await HttpPostJsonAsync<string, string>($"/api/SnowFlake/NextIds?count={count}", "");
                    break;
                }
                catch (Exception ex)
                {
                    // 记录错误日志
                    logger.LogError(ex, "获取多个SnowFlake ID失败，尝试次数：{Attempt}", attempt);

                    if (attempt == 3) throw;
                    await Task.Delay(1000);
                }
            }

            return result.Split(',').Select(long.Parse).ToList();
        }

        public Task SetId(long datacenterId, long workerId)
        {
            throw new NotImplementedException();
        }
    }
}