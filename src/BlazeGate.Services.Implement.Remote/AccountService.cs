using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;

namespace BlazeGate.Services.Implement.Remote
{
    public class AccountService : AuthWebApi, IAccountService
    {
        public AccountService(IHttpClientFactory httpClientFactory, IAuthTokenStorageServices authTokenStorage, IConfiguration configuration) : base(httpClientFactory, authTokenStorage, configuration)
        {
            WebApiAddress = BlazeGateAddress;
        }

        public async Task<ApiResult<UserDto>> GetUser(string serviceName)
        {
            return await HttpPostJsonAsync<string, ApiResult<UserDto>>($"/api/Account/GetUser?serviceName={serviceName}", "");
        }

        public async Task<ApiResult<AuthTokenDto>> Login(LoginParam param)
        {
            return await HttpPostJsonAsync<LoginParam, ApiResult<AuthTokenDto>>($"/api/Account/Login", param);
        }

        public async Task<ApiResult<string>> Logout(string serviceName, AuthTokenDto? authToken)
        {
            return await HttpPostJsonAsync<AuthTokenDto, ApiResult<string>>($"/api/Account/Logout?serviceName={serviceName}", authToken);
        }

        public async Task<ApiResult<AuthTokenDto>> RefreshToken(string serviceName, AuthTokenDto? authToken)
        {
            return await HttpPostJsonAsync<AuthTokenDto, ApiResult<AuthTokenDto>>($"/api/Account/RefreshToken?serviceName={serviceName}", authToken);
        }

        public async Task<ApiResult<string>> ChangePassword(ChangePasswordParam param)
        {
            return await HttpPostJsonAsync<ChangePasswordParam, ApiResult<string>>($"/api/Account/ChangePassword", param);
        }
    }
}