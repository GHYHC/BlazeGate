using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Model.WebApi;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlazeGate.Services.Interface;
using AutoMapper;
using BlazeGate.Model.JwtBearer;
using BlazeGate.Common.Autofac;

namespace BlazeGate.Services.Implement
{
    public class UserRoleService : IUserRoleService, IScopeDenpendency
    {
        private readonly BlazeGateContext BlazeGateContext;
        private readonly IMapper mapper;

        public UserRoleService(BlazeGateContext BlazeGateContext, IMapper mapper)
        {
            this.BlazeGateContext = BlazeGateContext;
            this.mapper = mapper;
        }

        public async Task<ApiResult<PaginatedList<UserRoleInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, UserRoleQuery userParam)
        {
            List<UserRole> userRoles = await BlazeGateContext.UserRoles.AsNoTracking().Where(p => p.ServiceName == serviceName).ToListAsync();
            List<long> userIds = userRoles.Select(p => p.UserId).Distinct().ToList();
            List<long> roleIds = userRoles.Select(p => p.RoleId).Distinct().ToList();

            var where = PredicateBuilder.New<User>(true);
            where.And(x => userIds.Contains(x.Id));
            if (userParam.UserId != null)
                where.And(x => x.Id == userParam.UserId);
            if (!string.IsNullOrWhiteSpace(userParam.UserName))
                where.And(x => x.UserName.Contains(userParam.UserName));
            if (!string.IsNullOrWhiteSpace(userParam.Account))
                where.And(x => x.Account.Contains(userParam.Account));

            var source = BlazeGateContext.Users.AsNoTracking().Where(where).OrderByDescending(b => b.Id);
            var list = await PaginatedList<User>.CreateAsync(source, pageIndex, pageSize);

            PaginatedList<UserRoleInfo> response = new PaginatedList<UserRoleInfo>();
            response.PageSize = list.PageSize;
            response.PageIndex = list.PageIndex;
            response.Total = list.Total;
            response.DataList = new List<UserRoleInfo>();
            if (list.DataList.Count > 0)
            {
                List<Role> roles = await BlazeGateContext.Roles.AsNoTracking().Where(p => roleIds.Contains(p.Id)).ToListAsync();
                foreach (var user in list.DataList)
                {
                    List<long> userRoleIds = userRoles.Where(p => p.UserId == user.Id).Select(p => p.RoleId).Distinct().ToList();

                    UserRoleInfo userRoleInfo = new UserRoleInfo();
                    userRoleInfo.User = mapper.Map<UserDto>(user);
                    userRoleInfo.Roles = roles.Where(p => userRoleIds.Contains(p.Id)).ToList();
                    response.DataList.Add(userRoleInfo);
                }
            }

            return ApiResult<PaginatedList<UserRoleInfo>>.SuccessResult(response);
        }

        public async Task<ApiResult<int>> RemoveById(long userId, string serviceName)
        {
            int i = await BlazeGateContext.UserRoles.AsNoTracking().Where(p => p.UserId == userId && p.ServiceName == serviceName).ExecuteDeleteAsync();
            return ApiResult<int>.Result(i > 0, i);
        }

        public async Task<ApiResult<List<Role>>> GetRoleByServiceName(string serviceName)
        {
            var list = await BlazeGateContext.Roles.Where(p => p.ServiceName == serviceName).ToListAsync();
            return ApiResult<List<Role>>.SuccessResult(list);
        }

        public async Task<ApiResult<int>> SaveUserRole(string serviceName, UserRoleSave userRoleSave)
        {
            //如果是新增，需要判断是否已经存在
            if (userRoleSave.IsAdd)
            {
                var userRole = await BlazeGateContext.UserRoles.AsNoTracking().FirstOrDefaultAsync(p => p.ServiceName == userRoleSave.ServiceName && p.UserId == userRoleSave.UserId);
                if (userRole != null)
                {
                    return ApiResult<int>.FailResult("用户已存在");
                }
            }

            var serviceId = await BlazeGateContext.Services.Where(b => b.ServiceName == userRoleSave.ServiceName).Select(b => b.Id).FirstOrDefaultAsync();

            var tran = BlazeGateContext.Database.BeginTransaction();
            try
            {
                BlazeGateContext.UserRoles.Where(p => p.ServiceName == userRoleSave.ServiceName && p.UserId == userRoleSave.UserId).ExecuteDelete();

                foreach (var roleId in userRoleSave.RoleIds)
                {
                    UserRole userRole = new UserRole();
                    userRole.ServiceId = serviceId;
                    userRole.ServiceName = userRoleSave.ServiceName;
                    userRole.UserId = userRoleSave.UserId.Value;
                    userRole.RoleId = roleId;
                    BlazeGateContext.UserRoles.Add(userRole);
                }

                await BlazeGateContext.SaveChangesAsync();
                tran.Commit();
                return ApiResult<int>.SuccessResult(1);
            }
            catch (Exception)
            {
                tran.Rollback();
                return ApiResult<int>.FailResult("保存失败");
            }
        }
    }
}