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
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<ApiResult<PaginatedList<UserInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, UserQuery userParam)
        {
            return await userService.QueryByPage(serviceName, pageIndex, pageSize, userParam);
        }

        [HttpPost]
        public async Task<ApiResult<int>> ChangeUserEnabled(string serviceName, long userId, bool enabled)
        {
            return await userService.ChangeUserEnabled(serviceName, userId, enabled);
        }

        [HttpPost]
        public async Task<ApiResult<int>> RemoveById(string serviceName, long userId)
        {
            return await userService.RemoveById(serviceName, userId);
        }

        [HttpPost]
        public async Task<ApiResult<int>> SaveUser(string serviceName, UserSave userSave)
        {
            return await userService.SaveUser(serviceName, userSave);
        }
    }
}