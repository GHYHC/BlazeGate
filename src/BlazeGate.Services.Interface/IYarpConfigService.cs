using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IYarpConfigService
    {
        /// <summary>
        /// 更新当前的路由配置
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public Task<ApiResult<bool>> Update(AuthBaseInfo auth);

        /// <summary>
        /// 更新所有的路由配置
        /// </summary>
        /// <returns></returns>
        public Task<ApiResult<bool>> UpdateAll(AuthBaseInfo auth);
    }
}