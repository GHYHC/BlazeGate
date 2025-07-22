using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;

namespace BlazeGate.Services.Implement.Remote
{
    public class UserService : AuthWebApi, IUserService
    {
        public UserService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            WebApiAddress = BlazeGateAddress;
        }

        public async Task<ApiResult<int>> ChangeUserEnabled(string serviceName, long userId, bool enabled)
        {
            return await HttpPostJsonAsync<string, ApiResult<int>>($"/api/User/ChangeUserEnabled?serviceName={serviceName}&userId={userId}&enabled={enabled}", "");
        }

        public async Task<ApiResult<PaginatedList<UserInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, UserQuery userParam)
        {
            return await HttpPostJsonAsync<UserQuery, ApiResult<PaginatedList<UserInfo>>>($"/api/User/QueryByPage?serviceName={serviceName}&pageIndex={pageIndex}&pageSize={pageSize}", userParam);
        }

        public async Task<ApiResult<int>> RemoveById(string serviceName, long userId)
        {
            return await HttpPostJsonAsync<string, ApiResult<int>>($"/api/User/RemoveById?serviceName={serviceName}&userId={userId}", "");
        }

        public async Task<ApiResult<int>> SaveUser(string serviceName, UserSave userSave)
        {
            return await HttpPostJsonAsync<UserSave, ApiResult<int>>($"/api/User/SaveUser?serviceName={serviceName}", userSave);
        }
    }
}