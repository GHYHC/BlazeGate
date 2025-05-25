using AutoMapper;
using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.JwtBearer;
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
    public class UserService : IUserService, IScopeDenpendency
    {
        private readonly BlazeGateContext context;
        private readonly ISnowFlakeService snowFlake;
        private readonly IMapper mapper;

        public UserService(BlazeGateContext context, ISnowFlakeService snowFlake, IMapper mapper)
        {
            this.context = context;
            this.snowFlake = snowFlake;
            this.mapper = mapper;
        }

        public async Task<ApiResult<PaginatedList<UserInfo>>> QueryByPage(string serviceName, int pageIndex, int pageSize, UserQuery userParam)
        {
            var where = PredicateBuilder.New<User>(true);
            if (userParam.UserId != null)
                where.And(x => x.Id == userParam.UserId);
            if (!string.IsNullOrWhiteSpace(userParam.NameOrAccount))
                where.And(x => x.UserName.Contains(userParam.NameOrAccount) || x.Account.Contains(userParam.NameOrAccount));

            var source = context.Users.AsNoTracking().Where(where).OrderByDescending(b => b.Id);
            var list = await PaginatedList<User>.CreateAsync(source, pageIndex, pageSize);

            PaginatedList<UserInfo> paginatedList = new PaginatedList<UserInfo>();
            paginatedList.PageSize = list.PageSize;
            paginatedList.PageIndex = list.PageIndex;
            paginatedList.Total = list.Total;
            paginatedList.DataList = mapper.Map<List<User>, List<UserInfo>>(list.DataList);

            return ApiResult<PaginatedList<UserInfo>>.SuccessResult(paginatedList);
        }

        public async Task<ApiResult<int>> ChangeUserEnabled(string serviceName, long userId, bool enabled)
        {
            var result = await context.Users.Where(b => b.Id == userId).ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Enabled, t => enabled)
                    .SetProperty(t => t.UpdateTime, t => DateTime.Now));
            return ApiResult<int>.Result(result > 0, result);
        }

        public async Task<ApiResult<int>> RemoveById(string serviceName, long userId)
        {
            using var tran = context.Database.BeginTransaction();
            try
            {
                var result = await context.Users.Where(p => p.Id == userId).ExecuteDeleteAsync();
                await context.UserRoles.Where(p => p.UserId == userId).ExecuteDeleteAsync();
                await tran.CommitAsync();
                return ApiResult<int>.Result(result > 0, result);
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult<int>.FailResult(ex.Message);
            }
        }

        public async Task<ApiResult<int>> SaveUser(string serviceName, UserSave userSave)
        {
            //判断账号不能重复
            var where = PredicateBuilder.New<User>(true);
            where.And(x => x.Account == userSave.Account);
            if (userSave.Id != 0)
            {
                where.And(x => x.Id != userSave.Id);
            }
            if (await context.Users.AnyAsync(where))
            {
                return ApiResult<int>.FailResult("账号已存在");
            }

            //判断手机号不能重复
            var where2 = PredicateBuilder.New<User>(true);
            where2.And(x => x.Phone == userSave.Phone);
            if (userSave.Id != 0)
            {
                where2.And(x => x.Id != userSave.Id);
            }
            if (await context.Users.AnyAsync(where2))
            {
                return ApiResult<int>.FailResult("手机号已存在");
            }

            if (userSave.Id == 0)
            {
                var user = mapper.Map<UserSave, User>(userSave);
                user.Id = await snowFlake.NextId();
                user.CreateTime = DateTime.Now;
                user.UpdateTime = DateTime.Now;
                context.Users.Add(user);
            }
            else
            {
                var oldUser = await context.Users.Where(p => p.Id == userSave.Id).FirstOrDefaultAsync();
                if (oldUser == null)
                    return ApiResult<int>.FailResult("用户不存在");

                oldUser.UserName = userSave.UserName;
                oldUser.Account = userSave.Account;
                oldUser.Phone = userSave.Phone;
                oldUser.Enabled = userSave.Enabled;
                if (UserSave.DefaultPassword != userSave.Password)
                {
                    oldUser.Password = userSave.Password;
                }
                oldUser.Remark = userSave.Remark ?? "";
                oldUser.UserData = userSave.UserData ?? "";
                oldUser.UpdateTime = DateTime.Now;
            }
            var result = await context.SaveChangesAsync();
            return ApiResult<int>.Result(result > 0, result);
        }
    }
}