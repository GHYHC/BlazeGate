using BlazeGate.Common;
using BlazeGate.Model.Helper;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Implement;
using BlazeGate.Services.Interface;
using Consul;
using Microsoft.Extensions.DependencyInjection;

namespace BlazeGate.BackgroundService
{
    /// <summary>
    /// 雪花算法ID检查服务
    /// </summary>
    public class SnowFlakeIdCheck : Microsoft.Extensions.Hosting.BackgroundService
    {
        private const int IntervalSeconds = 60;
        private readonly ILogger<SnowFlakeIdCheck> logger;
        private readonly ISnowFlakeService snowFlakeService;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public SnowFlakeIdCheck(ILogger<SnowFlakeIdCheck> logger, ISnowFlakeService snowFlakeService, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.snowFlakeService = snowFlakeService;
            this.configuration = configuration;
            this.httpClient = httpClientFactory.CreateClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string[] clusterAddress = configuration.GetSection("BlazeGateClusterAddress").Get<string[]>() ?? new string[] { };

                //判断如果集群数量超过1024个节点，则报错
                if (clusterAddress.Length > 1024)
                {
                    throw new Exception("集群节点数量超过1024个，请检查配置");
                }

                //获取当前节点的雪花算法信息
                SnowFlakeInfo currentSnowFlakeInfo = await snowFlakeService.GetSnowFlakeInfo();

                logger.LogInformation($"当前节点的雪花算法ID为 {currentSnowFlakeInfo.DatacenterId}:{currentSnowFlakeInfo.WorkerId}");

                //获取其他节点的雪花算法信息
                List<SnowFlakeInfo> snowFlakeInfoList = new List<SnowFlakeInfo>();
                foreach (var item in clusterAddress)
                {
                    try
                    {
                        string url = StringHelper.CombineUrl(item, "api/SnowFlake/GetSnowFlakeInfo");
                        var snowFlakeInfo = await httpClient.HttpPostAsJsonAsync<string, SnowFlakeInfo>(url, "");
                        if (snowFlakeInfo != null && currentSnowFlakeInfo.Guid != snowFlakeInfo.Guid)
                        {
                            snowFlakeInfoList.Add(snowFlakeInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"获取节点 {item} 的雪花算法信息失败：{ex.Message}");
                    }
                }

                long datacenterId = currentSnowFlakeInfo.DatacenterId;
                long workerId = currentSnowFlakeInfo.WorkerId;

                //查找可用的ID
                for (int i = 0; i < 1024; i++)
                {
                    if (snowFlakeInfoList.Any(b => b.DatacenterId == datacenterId && b.WorkerId == workerId))
                    {
                        datacenterId = i / 32;
                        workerId = i % 32;
                    }
                    else
                    {
                        break;
                    }
                }

                if (datacenterId != currentSnowFlakeInfo.DatacenterId || workerId != currentSnowFlakeInfo.WorkerId)
                {
                    await snowFlakeService.SetId(datacenterId, workerId);
                    logger.LogInformation($"更新雪花算法ID为 {datacenterId}:{workerId}");
                }

                await Task.Delay(TimeSpan.FromSeconds(IntervalSeconds), stoppingToken);
            }
        }
    }
}