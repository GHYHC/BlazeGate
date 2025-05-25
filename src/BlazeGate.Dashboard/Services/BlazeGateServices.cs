using BlazeGate.Common;
using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.Helper;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;

namespace BlazeGate.Dashboard.Services
{
    public class BlazeGateServices : IBlazeGateServices, IScopeDenpendency
    {
        private readonly IConfiguration configuration;

        private readonly HttpClient httpClient;

        public BlazeGateServices(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// 添加目标节点
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> Destination_Add(DestinationInfo destination)
        {
            string url = StringHelper.CombineUrl(configuration["BlazeGate:BlazeGateAddress"], "api/Destination/Add");
            return await httpClient.HttpPostAsJsonAsync<DestinationInfo, ApiResult<bool>>(url, destination);
        }

        /// <summary>
        /// 删除目标节点
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> Destination_Remove(DestinationInfo destination)
        {
            string url = StringHelper.CombineUrl(configuration["BlazeGate:BlazeGateAddress"], "api/Destination/Remove");
            return await httpClient.HttpPostAsJsonAsync<DestinationInfo, ApiResult<bool>>(url, destination);
        }

        /// <summary>
        /// 获取目标节点
        /// </summary>
        /// <param name="qurey"></param>
        /// <returns></returns>
        public async Task<ApiResult<List<Destination>>> Destination_GetDestinations(DestinationQurey qurey)
        {
            string url = StringHelper.CombineUrl(configuration["BlazeGate:BlazeGateAddress"], "api/Destination/GetDestinations");
            return await httpClient.HttpPostAsJsonAsync<DestinationQurey, ApiResult<List<Destination>>>(url, qurey);
        }

        /// <summary>
        /// 获取目标节点健康状态
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        public async Task<ApiResult<List<HealthStateCount>>> Destination_GetHealthStateCounts(AuthBaseInfo auth)
        {
            string url = StringHelper.CombineUrl(configuration["BlazeGate:BlazeGateAddress"], "api/Destination/GetHealthStateCounts");
            return await httpClient.HttpPostAsJsonAsync<AuthBaseInfo, ApiResult<List<HealthStateCount>>>(url, auth);
        }

        /// <summary>
        /// 更新yarp配置
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> YarpConfig_Update(AuthBaseInfo auth)
        {
            string url = StringHelper.CombineUrl(configuration["BlazeGate:BlazeGateAddress"], "api/YarpConfig/Update");
            return await httpClient.HttpPostAsJsonAsync<AuthBaseInfo, ApiResult<bool>>(url, auth);
        }

        /// <summary>
        /// 更新所有yarp配置
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> YarpConfig_UpdateAll(AuthBaseInfo auth)
        {
            string url = StringHelper.CombineUrl(configuration["BlazeGate:BlazeGateAddress"], "api/YarpConfig/UpdateAll");
            return await httpClient.HttpPostAsJsonAsync<AuthBaseInfo, ApiResult<bool>>(url, auth);
        }
    }

    public interface IBlazeGateServices
    {
        /// <summary>
        /// 添加目标节点
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> Destination_Add(DestinationInfo destination);

        /// <summary>
        /// 删除目标节点
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> Destination_Remove(DestinationInfo destination);

        /// <summary>
        /// 获取目标节点
        /// </summary>
        /// <param name="qurey"></param>
        /// <returns></returns>
        Task<ApiResult<List<Destination>>> Destination_GetDestinations(DestinationQurey qurey);

        /// <summary>
        /// 获取目标节点健康状态
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        Task<ApiResult<List<HealthStateCount>>> Destination_GetHealthStateCounts(AuthBaseInfo auth);

        /// <summary>
        /// 更新yarp配置
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> YarpConfig_Update(AuthBaseInfo auth);

        /// <summary>
        /// 更新所有yarp配置
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> YarpConfig_UpdateAll(AuthBaseInfo auth);
    }
}