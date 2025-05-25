using BlazeGate.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement.Remote
{
    public static class BlazeGateRemoteExtension
    {
        /// <summary>
        /// 添加BlazeGate远程服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBlazeGateRemote(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<ISnowFlakeService, SnowFlakeService>();

            return services;
        }

        /// <summary>
        /// 添加雪花算法服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBlazeGateSnowFlake(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddScoped<ISnowFlakeService, SnowFlakeService>();

            return services;
        }
    }
}