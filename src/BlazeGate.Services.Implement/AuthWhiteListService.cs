using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement
{
    public class AuthWhiteListService : IAuthWhiteListService, IScopeDenpendency
    {
        private readonly BlazeGateContext context;
        private readonly ISnowFlakeService snowFlake;

        public AuthWhiteListService(BlazeGateContext context, ISnowFlakeService snowFlake)
        {
            this.context = context;
            this.snowFlake = snowFlake;
        }

        public async Task<ApiResult<List<AuthWhiteList>>> GetWhiteListByServiceName(string serviceName)
        {
            var list = await context.AuthWhiteLists.AsNoTracking().Where(b => b.ServiceName == serviceName).ToListAsync();
            return ApiResult<List<AuthWhiteList>>.SuccessResult(list);
        }

        public async Task<ApiResult<int>> SaveWhiteList(string serviceName, List<AuthWhiteList> authWhiteList)
        {
            var services = await context.Services.AsNoTracking().Where(b => b.ServiceName == serviceName).FirstOrDefaultAsync();
            if (services == null)
            {
                return ApiResult<int>.FailResult("服务不存在");
            }

            int result = 0;
            result += context.AuthWhiteLists.Where(b => b.ServiceName == serviceName).ExecuteDelete();
            if (authWhiteList != null && authWhiteList.Count > 0)
            {
                foreach (var item in authWhiteList)
                {
                    item.Id = await snowFlake.NextId();
                    item.ServiceId = services.Id;
                    item.ServiceName = services.ServiceName;
                    item.CreateTime = DateTime.Now;
                    item.UpdateTime = DateTime.Now;

                    context.AuthWhiteLists.Add(item);
                }

                result += await context.SaveChangesAsync();
            }

            return ApiResult<int>.Result(result > 0, result);
        }
    }
}