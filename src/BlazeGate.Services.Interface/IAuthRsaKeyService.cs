using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IAuthRsaKeyService
    {
        public Task<ApiResult<AuthRsaKey>> GetAuthRsaKeyByServiceName(string serviceName);

        public Task<ApiResult<int>> SaveAuthRsaKey(string serviceName, AuthRsaKey authRsaKey);

        public Task<ApiResult<RsaKey>> CreateKey();
    }
}