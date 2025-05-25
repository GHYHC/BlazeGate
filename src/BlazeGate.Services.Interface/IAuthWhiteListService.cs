using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IAuthWhiteListService
    {
        /// <summary>
        /// 根据服务名称获取白名单
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public Task<ApiResult<List<AuthWhiteList>>> GetWhiteListByServiceName(string serviceName);

        /// <summary>
        /// 保存白名单
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="whiteList"></param>
        /// <returns></returns>
        public Task<ApiResult<int>> SaveWhiteList(string serviceName, List<AuthWhiteList> authWhiteList);
    }
}