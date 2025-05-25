using BlazeGate.JwtBearer;
using BlazeGate.Model.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlazeGate.Authorization
{
    public class RBACHandler : AuthorizationHandler<RBACRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RBACRequirement requirement)
        {
            // 判断是否认证
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                DefaultHttpContext defaultHttp = context.GetCurrentHttpContext();

                defaultHttp.Request.GetServiceInfo(out string serviceName, out string path);

                if (!string.IsNullOrWhiteSpace(serviceName))
                {
                    var user = context.User.GetUser();

                    IRBACService IRBACService = defaultHttp.RequestServices.GetService<IRBACService>();

                    if (await IRBACService.IsAuthorized(user.Account, serviceName, path))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
            else
            {
                context.Fail();
            }
        }
    }
}