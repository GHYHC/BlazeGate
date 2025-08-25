using BlazeGate.Model.Helper;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BlazeGate.AspNetCore
{
    internal class BlazeGateService : IHostedService
    {
        private readonly ILogger<BlazeGateService> logger;
        private readonly IHostApplicationLifetime lifetime;
        private readonly HttpClient httpClient;
        private readonly BlazeGateOptions blazeGateOptions;

        public BlazeGateService(IOptions<BlazeGateOptions> options, ILogger<BlazeGateService> logger, IHttpClientFactory httpClientFactory, IHostApplicationLifetime lifetime)
        {
            this.logger = logger;
            this.lifetime = lifetime;
            this.httpClient = httpClientFactory.CreateClient();
            this.blazeGateOptions = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                DestinationInfo destinationInfo = new DestinationInfo();
                destinationInfo.ServiceName = blazeGateOptions.ServiceName;
                destinationInfo.Token = blazeGateOptions.Token;
                destinationInfo.Address = blazeGateOptions.Address;

                string url = StringHelper.CombineUrl(blazeGateOptions.BlazeGateAddress, "api/Destination/Add");
                var response = await httpClient.PostAsJsonAsync(url, destinationInfo);
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>();
                if (result.Success)
                {
                    logger.LogInformation($"服务注册成功");
                }
                else
                {
                    logger.LogError($"服务注册失败：{result.Msg}");

                    //抛出异常
                    throw new Exception($"服务注册失败：{result.Msg}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"服务注册异常：{ex.Message}");
                //抛出异常
                throw new Exception($"服务注册异常：{ex.Message}");
            }

            //应用停止时注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                DestinationRemove().Wait();
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }

        public async Task DestinationRemove()
        {
            try
            {
                DestinationInfo destinationInfo = new DestinationInfo();
                destinationInfo.ServiceName = blazeGateOptions.ServiceName;
                destinationInfo.Token = blazeGateOptions.Token;
                destinationInfo.Address = blazeGateOptions.Address;

                string url = StringHelper.CombineUrl(blazeGateOptions.BlazeGateAddress, "api/Destination/Remove");
                var response = await httpClient.PostAsJsonAsync(url, destinationInfo);
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>();
                if (result.Success)
                {
                    logger.LogInformation($"服务注销成功");
                }
                else
                {
                    logger.LogError($"服务注销失败：{result.Msg}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"服务注销失败：{ex.Message}");
            }
        }
    }
}