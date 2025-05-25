using BlazeGate.Model.EFCore;
using BlazeGate.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BlazeGate.AuthWhiteList
{
    public class AuthWhiteListMiddleware
    {
        private readonly RequestDelegate next;

        public AuthWhiteListMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cache = context.RequestServices.GetService<IMemoryCache>();
            var dbContext = context.RequestServices.GetRequiredService<BlazeGateContext>();

            // 获取服务名和路径
            context.Request.GetServiceInfo(out string serviceName, out string path);

            // 获取白名单
            List<string> addressList = await cache.GetOrCreateAsync($"AuthWhiteList-{serviceName}", async entry =>
              {
                  entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                  return await dbContext.AuthWhiteLists.Where(s => s.ServiceName.ToLower() == serviceName).Select(s => s.Address.ToLower()).ToListAsync();
              }) ?? new List<string>();

            // 判断地址是否在白名单中(白名单后带有*则只匹配前面的)
            bool isWhiteList = addressList.Any(address => (address.EndsWith('*') && path.StartsWith(address.TrimEnd('*'))) || address == path);

            // 移除授权验证
            if (isWhiteList)
            {
                var endpoint = context.GetEndpoint();
                if (endpoint != null)
                {
                    var metadata = endpoint.Metadata.Where(m => m.GetType() != typeof(AuthorizeAttribute)).ToList();
                    var newEndpoint = new Endpoint(
                        endpoint.RequestDelegate,
                        new EndpointMetadataCollection(metadata),
                        endpoint.DisplayName
                    );
                    context.SetEndpoint(newEndpoint);
                }
            }

            await next(context);
        }
    }

    public static class AuthWhiteListMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthWhiteList(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthWhiteListMiddleware>();
        }
    }
}