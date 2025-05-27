using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazeGate.WebApi.Sample.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string GetHeader()
        {
            //获取头信息
            var dt = Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());

            string header = $"【真实IP】：{HttpContext.Connection.LocalIpAddress?.ToString()}:{HttpContext.Connection.RemotePort}\r\n";
            foreach (var key in dt.Keys)
            {
                header += $"【{key}】：{dt[key]} \r\n";
            }

            return header;
        }
    }
}