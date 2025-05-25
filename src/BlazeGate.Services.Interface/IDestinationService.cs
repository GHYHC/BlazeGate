using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IDestinationService
    {
        /// <summary>
        /// 添加目标
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public Task<ApiResult<bool>> Add(DestinationInfo destination);

        /// <summary>
        /// 删除目标
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public Task<ApiResult<bool>> Remove(DestinationInfo destination);

        /// <summary>
        /// 获取目标
        /// </summary>
        /// <param name="qurey"></param>
        /// <returns></returns>
        public Task<ApiResult<List<Destination>>> GetDestinations(DestinationQurey qurey);

        /// <summary>
        /// 获取健康状态统计
        /// </summary>
        /// <param name="qureys"></param>
        /// <returns></returns>
        public Task<ApiResult<List<HealthStateCount>>> GetHealthStateCounts(AuthBaseInfo auth);
    }
}