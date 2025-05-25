using AltairCA.Blazor.WebAssembly.Cookie;
using BlazeGate.Model.JwtBearer;
using BlazeGate.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.RBAC.Components.Extensions.AuthTokenStorage
{
    public class AuthTokenCookieStorageServices : IAuthTokenStorageServices
    {
        private readonly IAltairCABlazorCookieUtil cookieUtil;

        private readonly string key = "AuthToken";

        public AuthTokenCookieStorageServices(IAltairCABlazorCookieUtil cookieUtil)
        {
            this.cookieUtil = cookieUtil;
        }

        public async Task<AuthTokenDto> GetAuthToken()
        {
            return await cookieUtil.GetValueAsync<AuthTokenDto>(key);
        }

        public async Task SetAuthToken(AuthTokenDto authToken)
        {
            await cookieUtil.SetValueAsync(key, authToken);
        }
    }
}