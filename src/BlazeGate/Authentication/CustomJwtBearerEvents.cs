using BlazeGate.Authorization;
using BlazeGate.JwtBearer;
using BlazeGate.Model.EFCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace BlazeGate.Authentication
{
    public class CustomJwtBearerEvents : AppJwtBearerEvents
    {
        private readonly BlazeGateContext blazeGateContext;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<CustomJwtBearerEvents> logger;

        public CustomJwtBearerEvents(BlazeGateContext blazeGateContext, IMemoryCache memoryCache, ILogger<CustomJwtBearerEvents> logger)
        {
            this.blazeGateContext = blazeGateContext;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        public override async Task MessageReceived(MessageReceivedContext context)
        {
            await base.MessageReceived(context);

            context.HttpContext.Request.GetServiceInfo(out string serviceName, out string path);

            //如果是api请求，则从query中获取serviceName参数
            if ("api".Equals(serviceName))
            {
                serviceName = context.HttpContext.Request.Query["serviceName"];

                //如果serviceName参数为空，则从头部获取serviceName参数
                if (string.IsNullOrEmpty(serviceName))
                {
                    serviceName = context.HttpContext.Request.Headers["serviceName"].ToString();
                }
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                return;
            }

            //从缓存中获取RSA公钥
            var rsaSecurityPublicKey = await memoryCache.GetOrCreateAsync<RSAParameters?>($"PublicKey_{serviceName}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1); // 缓存1分钟

                string publicKey = await blazeGateContext.AuthRsaKeys.AsNoTracking().Where(s => s.ServiceName.ToLower() == serviceName.ToLower()).Select(b => b.PublicKey).FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(publicKey))
                {
                    try
                    {
                        return RsaParametersHelper.PemToRsaParameters(publicKey);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"解析公钥失败，ServiceName: {serviceName}");
                        return null;
                    }
                }

                return null;
            });

            if (rsaSecurityPublicKey != null)
            {
                //设置TokenValidationParameters的IssuerSigningKey为RSA公钥
                context.Options.TokenValidationParameters.IssuerSigningKey = new RsaSecurityKey(rsaSecurityPublicKey.Value);
            }
        }
    }
}