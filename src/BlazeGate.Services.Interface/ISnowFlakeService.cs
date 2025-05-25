using BlazeGate.Model.WebApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface ISnowFlakeService
    {
        /// <summary>
        /// 获取下一个id
        /// </summary>
        /// <returns></returns>
        Task<long> NextId();

        /// <summary>
        /// 获取多个id
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<List<long>> NextIds(int count);

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        Task<SnowFlakeInfo> GetSnowFlakeInfo();

        /// <summary>
        /// 设置数据中心ID和工作ID
        /// </summary>
        /// <param name="datacenterId"></param>
        /// <param name="workerId"></param>
        /// <returns></returns>
        Task SetId(long datacenterId, long workerId);
    }
}