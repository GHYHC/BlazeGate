using BlazeGate.Model.JwtBearer;
using BlazeGate.Services.Interface;
using Blazored.SessionStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.RBAC.Components.Extensions.AuthTokenStorage
{
    public class AuthTokenSessionStorageServices : IAuthTokenStorageServices
    {
        private readonly string key = "AuthToken";
        private readonly ISessionStorageService sessionStorage;

        public AuthTokenSessionStorageServices(ISessionStorageService sessionStorage)
        {
            this.sessionStorage = sessionStorage;
        }

        public async Task<AuthTokenDto> GetAuthToken()
        {
            return await sessionStorage.GetItemAsync<AuthTokenDto>(key);
        }

        public async Task SetAuthToken(AuthTokenDto authToken)
        {
            await sessionStorage.SetItemAsync(key, authToken);
        }
    }
}