using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.Policies;
using BlazeGate.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement
{
    public class DbInitializerService : IDbInitializer, IScopeDenpendency
    {
        private readonly BlazeGateContext context;
        private readonly ISnowFlakeService snowFlake;

        public DbInitializerService(BlazeGateContext context, ISnowFlakeService snowFlake)
        {
            this.context = context;
            this.snowFlake = snowFlake;
        }

        public async Task Initialize()
        {
            //自动迁移数据库
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
    }
}