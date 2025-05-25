using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlazeGate.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SnowFlakeController : Controller
    {
        private readonly ISnowFlakeService snowFlake;

        public SnowFlakeController(ISnowFlakeService snowFlake)
        {
            this.snowFlake = snowFlake;
        }

        [HttpGet]
        [HttpPost]
        public async Task<string> NextId()
        {
            long id = await snowFlake.NextId();
            return id.ToString();
        }

        [HttpGet]
        [HttpPost]
        public async Task<string> NextIds(int count)
        {
            var ids = await snowFlake.NextIds(count);
            return string.Join(",", ids);
        }

        [HttpGet]
        [HttpPost]
        public async Task<SnowFlakeInfo> GetSnowFlakeInfo()
        {
            return await snowFlake.GetSnowFlakeInfo();
        }
    }
}