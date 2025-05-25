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
    public interface IRoleService
    {
        /// <summary>
        /// 分页查询角色
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userParam"></param>
        /// <returns></returns>
        public Task<ApiResult<PaginatedList<RolePageInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, RolePageQuery query);

        /// <summary>
        /// 根据Id删除角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<ApiResult<int>> RemoveById(long roleId, string serviceName);

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="roleSave"></param>
        /// <returns></returns>
        public Task<ApiResult<int>> SaveRole(string serviceName,RoleSave roleSave);
    }
}