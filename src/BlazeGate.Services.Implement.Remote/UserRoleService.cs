using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;

namespace BlazeGate.Services.Implement.Remote
{
    public class UserRoleService : AuthWebApi, IUserRoleService
    {
        public UserRoleService(IHttpClientFactory httpClientFactory, IAuthTokenStorageServices authTokenStorage, IConfiguration configuration) : base(httpClientFactory, authTokenStorage, configuration)
        {
            WebApiAddress = BlazeGateAddress;
        }

        public async Task<ApiResult<PaginatedList<UserRoleInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, UserRoleQuery userParam)
        {
            return await HttpPostJsonAsync<UserRoleQuery, ApiResult<PaginatedList<UserRoleInfo>>>($"/api/UserRole/QueryByPage?serviceName={serviceName}&pageIndex={pageIndex}&pageSize={pageSize}", userParam);
        }

        public async Task<ApiResult<int>> RemoveById(long userId, string serviceName)
        {
            return await HttpPostJsonAsync<string, ApiResult<int>>($"/api/UserRole/RemoveById?userId={userId}&serviceName={serviceName}", "");
        }

        public async Task<ApiResult<List<Role>>> GetRoleByServiceName(string serviceName)
        {
            return await HttpPostJsonAsync<string, ApiResult<List<Role>>>($"/api/UserRole/GetRoleByServiceName?serviceName={serviceName}", "");
        }

        public async Task<ApiResult<int>> SaveUserRole(string serviceName, UserRoleSave userRoleSave)
        {
            return await HttpPostJsonAsync<UserRoleSave, ApiResult<int>>($"/api/UserRole/SaveUserRole?serviceName={serviceName}", userRoleSave);
        }
    }
}