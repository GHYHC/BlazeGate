using BlazeGate.Dashboard.Models;
using BlazeGate.Model.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace BlazeGate.Dashboard.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IDistributedCache distributedCache;

        public AccountController(IConfiguration configuration, IDistributedCache distributedCache)
        {
            this.configuration = configuration;
            this.distributedCache = distributedCache;
        }

        [HttpPost]
        public async Task<ApiResult<string>> Login([FromBody] LoginParams loginParams)
        {
            // 使用单个缓存键存储登录错误次数
            string cacheKey = $"login:BlazeGateDashboardAttempts:{loginParams.UserName}";
            string attemptsStr = await distributedCache.GetStringAsync(cacheKey);
            int attempts = 0;

            // 如果有缓存记录，则解析尝试次数
            if (!string.IsNullOrEmpty(attemptsStr) && int.TryParse(attemptsStr, out attempts))
            {
                // 如果尝试次数达到或超过3次，返回账号锁定信息
                if (attempts >= 3)
                {
                    return new ApiResult<string> { Code = 0, Success = false, Msg = "登录失败次数过多，账号已被锁定，请1分钟后再试" };
                }
            }

            string username = configuration["Login:Username"];
            string password = configuration["Login:Password"];
            if (loginParams.UserName == username && loginParams.Password == password)
            {
                // 登录成功，清除错误计数
                await distributedCache.RemoveAsync(cacheKey);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, loginParams.UserName));
                await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

                return new ApiResult<string> { Code = 1, Success = true, Msg = "登录成功" };
            }
            else
            {
                // 登录失败，增加错误次数，并设置1分钟过期时间
                attempts++;
                await distributedCache.SetStringAsync(cacheKey, attempts.ToString(), new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                });

                return new ApiResult<string> { Code = 0, Success = false, Msg = "用户名或密码错误" };
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return new RedirectResult("/Account/Login");
        }
    }
}