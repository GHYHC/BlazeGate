using BlazeGate.Model.JwtBearer;
using BlazeGate.Services.Interface;

namespace BlazeGate.RBAC.Components.Extensions.AuthTokenStorage
{
    public class AuthTokenCookieStorageServices : IAuthTokenStorageServices
    {
        private readonly ICookieService cookieService;

        private readonly string key = "AuthToken";

        public AuthTokenCookieStorageServices(ICookieService cookieService)
        {
            this.cookieService = cookieService;
        }

        public async Task<AuthTokenDto> GetAuthToken()
        {
            return await cookieService.GetCookieAsync<AuthTokenDto>(key);
        }

        public async Task SetAuthToken(AuthTokenDto authToken)
        {
            await cookieService.SetCookieAsync(key, authToken);
        }
    }
}