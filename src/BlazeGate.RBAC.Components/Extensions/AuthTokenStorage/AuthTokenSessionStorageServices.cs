using BlazeGate.Model.JwtBearer;
using BlazeGate.Services.Interface;

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