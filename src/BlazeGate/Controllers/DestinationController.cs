using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using BlazeGate.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazeGate.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(ServiceAuthFilter))]//服务授权验证
    public class DestinationController : ControllerBase
    {
        private readonly IDestinationService destinationService;

        public DestinationController(IDestinationService destinationService)
        {
            this.destinationService = destinationService;
        }

        /// <summary>
        /// 添加目标
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<bool>> Add(DestinationInfo destination)
        {
            return await destinationService.Add(destination);
        }

        /// <summary>
        /// 删除目标
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<bool>> Remove(DestinationInfo destination)
        {
            return await destinationService.Remove(destination);
        }

        /// <summary>
        /// 获取目标
        /// </summary>
        /// <param name="qurey"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<List<Destination>>> GetDestinations(DestinationQurey qurey)
        {
            return await destinationService.GetDestinations(qurey);
        }

        /// <summary>
        /// 获取健康状态统计
        /// </summary>
        /// <param name="qureys"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<List<HealthStateCount>>> GetHealthStateCounts(AuthBaseInfo auth)
        {
            return await destinationService.GetHealthStateCounts(auth);
        }
    }
}