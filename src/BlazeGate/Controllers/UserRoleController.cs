using BlazeGate.Model.EFCore;
using BlazeGate.Model.Helper;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using BlazeGate.Authorization;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Role = BlazeGate.Model.EFCore.Role;

namespace BlazeGate.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(RBACAuthFilter))]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            this.userRoleService = userRoleService;
        }

        [HttpPost]
        public Task<ApiResult<PaginatedList<UserRoleInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, UserRoleQuery userParam)
        {
            return userRoleService.QueryByPage(serviceName, pageIndex, pageSize, userParam);
        }

        [HttpPost]
        public Task<ApiResult<int>> RemoveById(long userId, string serviceName)
        {
            return userRoleService.RemoveById(userId, serviceName);
        }

        [HttpPost]
        public Task<ApiResult<List<Role>>> GetRoleByServiceName(string serviceName)
        {
            return userRoleService.GetRoleByServiceName(serviceName);
        }

        [HttpPost]
        public Task<ApiResult<int>> SaveUserRole(string serviceName, UserRoleSave userRoleSave)
        {
            return userRoleService.SaveUserRole(serviceName, userRoleSave);
        }
    }
}