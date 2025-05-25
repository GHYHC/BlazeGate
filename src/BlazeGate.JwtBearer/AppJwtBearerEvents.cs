using BlazeGate.Model.JwtBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BlazeGate.JwtBearer
{
    public class AppJwtBearerEvents : JwtBearerEvents
    {
        public override Task MessageReceived(MessageReceivedContext context)
        {
            AuthTokenDto authToken = context.Request.Cookies.GetAuthToken();
            context.Token = authToken?.AccessToken;
            return Task.CompletedTask;
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            //添加标记，使前端知晓access token过期，可以使用refresh token了
            if (context.Exception is SecurityTokenExpiredException)
            {
                //判断请求的方法是否加上Authorize的特性
                var endpoint = context.HttpContext.GetEndpoint();
                if (endpoint != null && endpoint.Metadata.Any(x => x.GetType() == typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute)))
                {
                    context.Response.Headers.Add("x-access-token", "expired");
                }
            }

            return Task.CompletedTask;
        }
    }
}
