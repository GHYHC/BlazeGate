using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazeGate.Filter
{
    /// <summary>
    /// 服务授权验证过滤器
    /// </summary>
    public class ServiceAuthFilter : IAsyncActionFilter, IScopeDenpendency
    {
        private readonly IMemoryCache memoryCache;
        private readonly BlazeGateContext dbContext;

        public ServiceAuthFilter(IMemoryCache memoryCache, BlazeGateContext dbContext)
        {
            this.memoryCache = memoryCache;
            this.dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. 查找AuthBaseInfo参数
            var authBaseInfo = context.ActionArguments.Values
                .OfType<AuthBaseInfo>()
                .FirstOrDefault();

            // 如果没有AuthBaseInfo参数，返回参数错误
            if (authBaseInfo == null)
            {
                context.Result = new ObjectResult(new ApiResult<string>() { Code = 400, Msg = "缺少授权信息参数" })
                {
                    StatusCode = 400
                };
                return;
            }

            // 验证参数
            if (string.IsNullOrWhiteSpace(authBaseInfo.ServiceName) || string.IsNullOrWhiteSpace(authBaseInfo.Token))
            {
                context.Result = new ObjectResult(new ApiResult<string>() { Code = 400, Msg = "缺少授权信息参数" })
                {
                    StatusCode = 400
                };
                return;
            }

            // 使用缓存减少数据库查询
            bool isValidService = await memoryCache.GetOrCreateAsync($"ServiceAuth_{authBaseInfo.ServiceName}_{authBaseInfo.Token}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1); // 缓存1分钟

                var service = await dbContext.Services
                    .AsNoTracking()
                    .Where(s => s.ServiceName == authBaseInfo.ServiceName)
                    .FirstOrDefaultAsync();

                if (service == null)
                    return false;

                // 查找对应的Token配置
                return service.Token == authBaseInfo.Token;
            });

            if (!isValidService)
            {
                context.Result = new ObjectResult(new ApiResult<string>() { Code = 403, Msg = "缺少授权信息参数" })
                {
                    StatusCode = 403
                };
                return;
            }

            // 验证通过，继续执行
            await next();
        }
    }
}