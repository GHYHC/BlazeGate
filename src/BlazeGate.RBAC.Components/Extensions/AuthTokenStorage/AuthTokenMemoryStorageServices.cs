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
    public class AuthTokenMemoryStorageServices : IAuthTokenStorageServices
    {
        private AuthTokenDto authToken;

        public AuthTokenMemoryStorageServices()
        {
        }

        public async Task<AuthTokenDto> GetAuthToken()
        {
            return authToken;
        }

        public async Task SetAuthToken(AuthTokenDto authToken)
        {
            this.authToken = authToken;
        }
    }
}