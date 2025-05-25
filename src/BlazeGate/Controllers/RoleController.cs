using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using BlazeGate.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazeGate.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(RBACAuthFilter))]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RoleController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpPost]
        public async Task<ApiResult<PaginatedList<RolePageInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, RolePageQuery query)
        {
            return await roleService.QueryByPage(serviceName, pageIndex, pageSize, query);
        }

        [HttpPost]
        public async Task<ApiResult<int>> RemoveById(long roleId, string serviceName)
        {
            return await roleService.RemoveById(roleId, serviceName);
        }

        [HttpPost]
        public async Task<ApiResult<int>> SaveRole(string serviceName, RoleSave roleSave)
        {
            return await roleService.SaveRole(serviceName, roleSave);
        }
    }
}