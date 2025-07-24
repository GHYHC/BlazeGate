using BlazeGate.Model.Culture;
using BlazeGate.RBAC.Components.Extensions.AppCultureStorage;
using BlazeGate.RBAC.Components.Extensions.Authentication;
using BlazeGate.RBAC.Components.Extensions.AuthTokenStorage;
using BlazeGate.RBAC.Components.Extensions.Menu;
using BlazeGate.RBAC.locales;
using BlazeGate.Services.Implement.Remote;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

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
            //覆盖AntDesign的Locale
            LocaleHelp.OverrideAntDesignLocaleByAssembly(typeof(BlazeGateRBACExtension).Assembly);

            services.AddBlazeGateRemote();

            //添加本地化支持
            services.AddLocalization();

            //添加页面服务
            services.AddScoped<IPageDataStorageServices, PageDataStorageServices>();
            //添加菜单服务
            services.AddScoped<IMenuServices, MenuServices>();

            //添加Cookie服务
            services.AddScoped<ICookieService, CookieService>();
            //添加本地存储服务
            services.AddScoped<ILocalStorageService, LocalStorageService>();
            //添加Session存储服务
            services.AddScoped<ISessionStorageService, SessionStorageService>();

            //添加Token存储服务
            if (storageEnum == AuthTokenStorageEnum.LocalStorage)
            {
                services.AddScoped<IAuthTokenStorageServices, AuthTokenLocalStorageServices>();
            }
            else if (storageEnum == AuthTokenStorageEnum.SessionStorage)
            {
                services.AddScoped<IAuthTokenStorageServices, AuthTokenSessionStorageServices>();
            }
            else if (storageEnum == AuthTokenStorageEnum.Memory)
            {
                services.AddScoped<IAuthTokenStorageServices, AuthTokenMemoryStorageServices>();
            }
            else if (storageEnum == AuthTokenStorageEnum.Cookie)
            {
                services.AddScoped<IAuthTokenStorageServices, AuthTokenCookieStorageServices>();
            }
            else if (storageEnum == AuthTokenStorageEnum.File)
            {
                services.AddScoped<IAuthTokenStorageServices, AuthTokenFileStorageServices>();
            }

            //添加应用程序文化信息存储服务
            services.AddScoped<IAppCultureStorageService, AppCultureCookieStorageServices>();

            services.AddAuthorizationCore();
            services.AddCascadingAuthenticationState();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            return services;
        }

        /// <summary>
        /// 设置应用程序的文化信息
        /// </summary>
        public static async Task<WebAssemblyHost> SetCultureAsync(this WebAssemblyHost host)
        {
            var appCultureStorageService = host.Services.GetRequiredService<IAppCultureStorageService>();

            AppCultureInfo appCultureInfo = await appCultureStorageService.GetAppCulture();

            if (appCultureInfo != null)
            {
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(appCultureInfo.Culture);
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(appCultureInfo.UICulture);
            }

            return host;
        }

        /// <summary>
        /// 设置应用程序的文化信息
        /// </summary>
        public static IApplicationBuilder SetCultureAsync(this IApplicationBuilder app, string[] languages = null)
        {
            if (languages == null || languages.Length <= 0)
            {
                languages = LanguageOptions.Languages;
            }

            // 使用以根据客户端提供的信息自动设置请求的文化信息
            app.UseRequestLocalization(new RequestLocalizationOptions()
                .SetDefaultCulture(languages[0])
                .AddSupportedCultures(languages)
                .AddSupportedUICultures(languages)
            );

            return app;
        }
    }
}