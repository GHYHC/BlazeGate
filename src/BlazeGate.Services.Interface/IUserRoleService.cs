using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Model.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazeGate.Model.EFCore;

namespace BlazeGate.Services.Interface
{
    public interface IUserRoleService
    {
        /// <summary>
        /// 分页查询用户角色
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userParam"></param>
        /// <returns></returns>
        public Task<ApiResult<PaginatedList<UserRoleInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, UserRoleQuery userParam);

        /// <summary>
        /// 根据用户ID删除用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<ApiResult<int>> RemoveById(long userId, string serviceName);

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public Task<ApiResult<List<Role>>> GetRoleByServiceName(string serviceName);

        /// <summary>
        /// 保存用户角色
        /// </summary>
        /// <param name="userRoles"></param>
        /// <returns></returns>
        public Task<ApiResult<int>> SaveUserRole(string serviceName, UserRoleSave userRoleSave);
    }
}