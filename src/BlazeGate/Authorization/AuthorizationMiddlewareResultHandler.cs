using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using BlazeGate.Model.WebApi;

namespace BlazeGate.Authorization
{
    public class AuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            //这里授权是否成功
            if (!authorizeResult.Succeeded)
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new ApiResult<string>() { Code = 401, Msg = "身份验证不通过", Data = string.Empty });
                }
                else
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new ApiResult<string>() { Code = 403, Msg = "没有权限", Data = string.Empty });
                }
                //注意一定要return 在这里短路管道 不要走到next 否则线程会进入后续管道 到达action中
                return;
            }
            //如果授权成功 继续执行后续的中间件 记住一定记得next 否则会管道会短路
            await next(context);
        }
    }
}
