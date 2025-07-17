using BlazeGate.Model.JwtBearer;
using BlazeGate.Services.Interface;

namespace BlazeGate.RBAC.Components.Extensions.AuthTokenStorage
{
    public class AuthTokenLocalStorageServices : IAuthTokenStorageServices
    {
        private readonly ILocalStorageService localStorage;

        private readonly string key = "AuthToken";

        public AuthTokenLocalStorageServices(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task<AuthTokenDto> GetAuthToken()
        {
            return await localStorage.GetItemAsync<AuthTokenDto>(key);
        }

        public async Task SetAuthToken(AuthTokenDto authToken)
        {
            await localStorage.SetItemAsync(key, authToken);
        }
    }
}