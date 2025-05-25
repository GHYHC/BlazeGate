using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Services.Interface;
using BlazeGate.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazeGate.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(ServiceAuthFilter))]//服务授权验证
    public class YarpConfigController : ControllerBase
    {
        private readonly IYarpConfigService yarpConfigService;

        public YarpConfigController(IYarpConfigService yarpConfigService)
        {
            this.yarpConfigService = yarpConfigService;
        }

        [HttpPost]
        public async Task<ApiResult<bool>> Update(AuthBaseInfo auth)
        {
            return await yarpConfigService.Update(auth);
        }

        [HttpPost]
        public async Task<ApiResult<bool>> UpdateAll(AuthBaseInfo auth)
        {
            return await yarpConfigService.UpdateAll(auth);
        }
    }
}