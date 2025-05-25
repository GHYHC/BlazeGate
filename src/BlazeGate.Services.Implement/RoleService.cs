using Azure;
using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement
{
    public class RoleService : IRoleService, IScopeDenpendency
    {
        private readonly BlazeGateContext BlazeGateContext;
        private readonly ISnowFlakeService snowFlake;

        public RoleService(BlazeGateContext BlazeGateContext, ISnowFlakeService snowFlake)
        {
            this.BlazeGateContext = BlazeGateContext;
            this.snowFlake = snowFlake;
        }

        public async Task<ApiResult<PaginatedList<RolePageInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, RolePageQuery query)
        {
            var where = PredicateBuilder.New<Role>(true);
            where.And(x => x.ServiceName == serviceName);
            if (query.RoleId != null)
            {
                where.And(x => x.Id == query.RoleId);
            }
            if (!string.IsNullOrWhiteSpace(query.RoleName))
            {
                where.And(x => x.RoleName.Contains(query.RoleName));
            }

            var source = BlazeGateContext.Roles.AsNoTracking().Where(where).OrderByDescending(b => b.Id);
            var list = await PaginatedList<Role>.CreateAsync(source, pageIndex, pageSize);

            PaginatedList<RolePageInfo> response = new PaginatedList<RolePageInfo>();
            response.PageSize = list.PageSize;
            response.PageIndex = list.PageIndex;
            response.Total = list.Total;
            response.DataList = new List<RolePageInfo>();

            if (list.DataList.Count > 0)
            {
                var roleIds = list.DataList.Select(p => p.Id).Distinct().ToList();

                var rolePages = BlazeGateContext.RolePages.AsNoTracking().Where(p => roleIds.Contains(p.RoleId) && p.ServiceName == serviceName).ToList();

                var pages = BlazeGateContext.Pages.AsNoTracking().Where(p => rolePages.Select(x => x.PageId).Contains(p.Id)).ToList();

                foreach (var item in list.DataList)
                {
                    RolePageInfo rolePageInfo = new RolePageInfo();
                    rolePageInfo.Role = item;
                    rolePageInfo.Pages = pages.Where(p => rolePages.Where(x => x.RoleId == item.Id).Select(x => x.PageId).Contains(p.Id)).ToList();
                    response.DataList.Add(rolePageInfo);
                }
            }

            return ApiResult<PaginatedList<RolePageInfo>>.SuccessResult(response);
        }

        public async Task<ApiResult<int>> RemoveById(long roleId, string serviceName)
        {
            var tran = BlazeGateContext.Database.BeginTransaction();
            try
            {
                await BlazeGateContext.Roles.Where(p => p.Id == roleId && p.ServiceName == serviceName).ExecuteDeleteAsync();
                await BlazeGateContext.RolePages.Where(p => p.RoleId == roleId && p.ServiceName == serviceName).ExecuteDeleteAsync();
                tran.Commit();
                return ApiResult<int>.SuccessResult(1);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ApiResult<int>.FailResult(ex.Message);
            }
        }

        public async Task<ApiResult<int>> SaveRole(string serviceName, RoleSave roleSave)
        {
            if (roleSave.ServiceId == 0)
            {
                roleSave.ServiceId = await BlazeGateContext.Services.Where(b => b.ServiceName == roleSave.ServiceName).Select(b => b.Id).FirstOrDefaultAsync();
            }

            //判断角色是否重复如果是修改则排除自己
            var where = PredicateBuilder.New<Role>(true);
            where.And(x => x.ServiceId == roleSave.ServiceId);
            where.And(x => x.RoleName == roleSave.RoleName);
            if (roleSave.RoleId > 0)
            {
                where.And(x => x.Id != roleSave.RoleId);
            }
            var count = await BlazeGateContext.Roles.CountAsync(where);
            if (count > 0)
            {
                return ApiResult<int>.FailResult("角色名称重复");
            }

            var tran = BlazeGateContext.Database.BeginTransaction();
            try
            {
                long roleId = roleSave.RoleId;

                if (roleSave.RoleId == 0)
                {
                    //添加角色
                    Role role = new Role();
                    role.Id = await snowFlake.NextId();
                    role.RoleName = roleSave.RoleName;
                    role.ServiceId = roleSave.ServiceId;
                    role.ServiceName = roleSave.ServiceName;
                    role.Remark = roleSave.Remark ?? "";
                    role.CreateTime = DateTime.Now;
                    role.UpdateTime = DateTime.Now;
                    BlazeGateContext.Roles.Add(role);

                    roleId = role.Id;
                }
                else
                {
                    //修改角色
                    await BlazeGateContext.Roles
                       .Where(b => b.Id == roleSave.RoleId && b.ServiceId == roleSave.ServiceId)
                       .ExecuteUpdateAsync(s => s
                            .SetProperty(t => t.RoleName, t => roleSave.RoleName)
                            .SetProperty(t => t.Remark, t => roleSave.Remark ?? "")
                            .SetProperty(t => t.UpdateTime, t => DateTime.Now));

                    //删除角色页面
                    BlazeGateContext.RolePages.Where(p => p.RoleId == roleSave.RoleId && p.ServiceId == roleSave.ServiceId).ExecuteDelete();
                }

                //添加角色页面
                foreach (var item in roleSave.PageIds)
                {
                    RolePage rolePage = new RolePage();
                    rolePage.RoleId = roleId;
                    rolePage.PageId = long.Parse(item);
                    rolePage.ServiceId = roleSave.ServiceId;
                    rolePage.ServiceName = roleSave.ServiceName;
                    BlazeGateContext.RolePages.Add(rolePage);
                }
                var i = await BlazeGateContext.SaveChangesAsync();

                tran.Commit();
                return ApiResult<int>.SuccessResult(i);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return ApiResult<int>.FailResult(ex.Message);
            }
        }
    }
}