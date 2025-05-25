using BlazeGate.Common;
using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement
{
    public class AuthRsaKeyService : IAuthRsaKeyService, IScopeDenpendency
    {
        private readonly BlazeGateContext context;
        private readonly ISnowFlakeService snowFlake;

        public AuthRsaKeyService(BlazeGateContext context, ISnowFlakeService snowFlake)
        {
            this.context = context;
            this.snowFlake = snowFlake;
        }

        public async Task<ApiResult<AuthRsaKey>> GetAuthRsaKeyByServiceName(string serviceName)
        {
            var authRsaKey = await context.AuthRsaKeys.AsNoTracking().Where(b => b.ServiceName == serviceName).FirstOrDefaultAsync();
            return ApiResult<AuthRsaKey>.SuccessResult(authRsaKey);
        }

        public async Task<ApiResult<int>> SaveAuthRsaKey(string serviceName, AuthRsaKey authRsaKey)
        {
            var services = await context.Services.AsNoTracking().Where(b => b.ServiceName == serviceName).FirstOrDefaultAsync();
            if (services == null)
            {
                return ApiResult<int>.FailResult("服务不存在");
            }
            if (authRsaKey.Id > 0)
            {
                authRsaKey.ServiceId = services.Id;
                authRsaKey.ServiceName = services.ServiceName;
                authRsaKey.UpdateTime = DateTime.Now;
                context.AuthRsaKeys.Update(authRsaKey);
            }
            else
            {
                authRsaKey.Id = await snowFlake.NextId();
                authRsaKey.ServiceId = services.Id;
                authRsaKey.ServiceName = services.ServiceName;
                authRsaKey.CreateTime = DateTime.Now;
                authRsaKey.UpdateTime = DateTime.Now;
                context.AuthRsaKeys.Add(authRsaKey);
            }

            int result = await context.SaveChangesAsync();
            return ApiResult<int>.Result(result > 0, result);
        }

        public async Task<ApiResult<RsaKey>> CreateKey()
        {
            RSAParameters privateKey, publicKey;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    privateKey = rsa.ExportParameters(true);
                    publicKey = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            RsaKey model = new RsaKey();
            model.PublicKey = RsaParametersHelper.RsaParametersToPem(publicKey, false);
            model.PrivateKey = RsaParametersHelper.RsaParametersToPem(privateKey, true);

            return ApiResult<RsaKey>.SuccessResult(model);
        }
    }
}