using BlazeGate.Common.Autofac;
using BlazeGate.JwtBearer;
using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlazeGate.Authorization
{
    /// <summary>
    /// RBAC权限验证过滤器
    /// </summary>
    public class RBACAuthFilter : IAsyncActionFilter, IScopeDenpendency
    {
        private readonly IRBACService iRBACService;

        public RBACAuthFilter(IRBACService IRBACService)
        {
            iRBACService = IRBACService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //获取请求的serviceName参数
            string serviceName = context.HttpContext.Request.Query["serviceName"];
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                context.Result = new ObjectResult(new ApiResult<string>() { Code = 400, Msg = "缺少serviceName参数" })
                {
                    StatusCode = 400
                };
                return;
            }

            //获取请求的path
            string path = context.HttpContext.Request.Path.Value?.ToLower();

            //获取用户信息
            var user = context.HttpContext.User.GetUser();

            //判断是否有权限
            if (!await iRBACService.IsAuthorized(user.Account, serviceName, path))
            {
                context.Result = new ObjectResult(new ApiResult<string>() { Code = 403, Msg = "没有权限" })
                {
                    StatusCode = 403
                };
                return;
            }

            await next();
        }
    }
}