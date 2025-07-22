using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;

namespace BlazeGate.Services.Implement.Remote
{
    public class RoleService : AuthWebApi, IRoleService
    {
        public RoleService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            WebApiAddress = BlazeGateAddress;
        }

        public async Task<ApiResult<PaginatedList<RolePageInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, RolePageQuery query)
        {
            return await HttpPostJsonAsync<RolePageQuery, ApiResult<PaginatedList<RolePageInfo>>>($"/api/Role/QueryByPage?serviceName={serviceName}&pageIndex={pageIndex}&pageSize={pageSize}", query);
        }

        public async Task<ApiResult<int>> RemoveById(long roleId, string serviceName)
        {
            return await HttpPostJsonAsync<string, ApiResult<int>>($"/api/Role/RemoveById?roleId={roleId}&serviceName={serviceName}", "");
        }

        public async Task<ApiResult<int>> SaveRole(string serviceName, RoleSave roleSave)
        {
            return await HttpPostJsonAsync<RoleSave, ApiResult<int>>($"/api/Role/SaveRole?serviceName={serviceName}", roleSave);
        }
    }
}