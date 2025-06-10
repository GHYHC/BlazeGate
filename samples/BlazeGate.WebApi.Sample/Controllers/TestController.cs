using Microsoft.AspNetCore.Mvc;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazeGate.WebApi.Sample.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISnowFlakeService snowFlakeService;

        public TestController(ISnowFlakeService snowFlakeService)
        {
            this.snowFlakeService = snowFlakeService;
        }

        [HttpGet]
        public string GetHeader()
        {
            //获取头信息
            var dt = Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());

            string header = $"【真实IP】：{HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString()}:{HttpContext.Connection.RemotePort}\r\n";
            foreach (var key in dt.Keys)
            {
                header += $"【{key}】：{dt[key]} \r\n";
            }

            return header;
        }

        [HttpGet]
        public async Task GetId()
        {
            var ids = await snowFlakeService.NextIds(10);
            var id = await snowFlakeService.NextId();
        }
    }
}