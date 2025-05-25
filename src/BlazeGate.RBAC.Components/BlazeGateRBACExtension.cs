using AltairCA.Blazor.WebAssembly.Cookie.Framework;
using BlazeGate.RBAC.Components.Extensions.Authentication;
using BlazeGate.RBAC.Components.Extensions.AuthTokenStorage;
using BlazeGate.RBAC.Components.Extensions.Menu;
using BlazeGate.Services.Implement.Remote;
using BlazeGate.Services.Interface;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BlazeGate.RBAC.Components
{
    public static class BlazeGateRBACExtension
    {
        /// <summary>
        /// 添加RBAC服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builder"></param>
        /// <returns></returns>

        public static IServiceCollection AddBlazeGateRBAC(this IServiceCollection services, AuthTokenStorageEnum storageEnum = AuthTokenStorageEnum.Cookie)
        {
            services.AddBlazeGateRemote();

            //添加页面服务
            services.AddSingleton<IPageDataStorageServices, PageDataStorageServices>();
            //添加菜单服务
            services.AddScoped<IMenuServices, MenuServices>();

            //添加Token存储服务
            if (storageEnum == AuthTokenStorageEnum.LocalStorage)
            {
                services.AddBlazoredLocalStorageAsSingleton();
                services.AddSingleton<IAuthTokenStorageServices, AuthTokenLocalStorageServices>();
            }
            else if (storageEnum == AuthTokenStorageEnum.SessionStorage)
            {
                services.AddBlazoredSessionStorageAsSingleton();
                services.AddSingleton<IAuthTokenStorageServices, AuthTokenSessionStorageServices>();
            }
            else if (storageEnum == AuthTokenStorageEnum.Memory)
            {
                services.AddSingleton<IAuthTokenStorageServices, AuthTokenMemoryStorageServices>();
            }
            else if (storageEnum == AuthTokenStorageEnum.Cookie)
            {
                services.AddAltairCACookieService(options => { });
                services.AddSingleton<IAuthTokenStorageServices, AuthTokenCookieStorageServices>();
            }
            else if (storageEnum == AuthTokenStorageEnum.File)
            {
                services.AddSingleton<IAuthTokenStorageServices, AuthTokenFileStorageServices>();
            }

            services.AddAuthorizationCore();
            services.AddCascadingAuthenticationState();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            return services;
        }
    }
}