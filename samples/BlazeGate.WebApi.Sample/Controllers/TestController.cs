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

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string body = "";
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            return Ok($"请求长度：{body.Length}");
        }

        [HttpPost]
        public async Task<IActionResult> Error()
        {
            // 先让客户端/代理确定已经开始接收响应（减少被缓冲吞掉的概率）
            Response.StatusCode = StatusCodes.Status200OK;
            await Response.WriteAsync("start...");
            await Response.Body.FlushAsync();

            // 模拟一点处理时间
            await Task.Delay(200);

            // 关键：直接中断连接，制造转发层传输错误（更可能触发 IForwarderErrorFeature）
            HttpContext.Abort();

            // 这行一般到不了；只是为了满足编译器
            return new EmptyResult();
        }
    }
}