using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlazeGate.WebApi.Sample.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISnowFlakeService snowFlakeService;
        private readonly ILogger<TestController> logger;

        public TestController(ISnowFlakeService snowFlakeService, ILogger<TestController> logger)
        {
            this.snowFlakeService = snowFlakeService;
            this.logger = logger;
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
            logger.LogInformation(header);
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