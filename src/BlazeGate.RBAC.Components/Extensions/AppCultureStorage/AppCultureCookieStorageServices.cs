using BlazeGate.Model.Culture;
using BlazeGate.Model.JwtBearer;
using BlazeGate.RBAC.Components.Extensions.AuthTokenStorage;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Localization;
using System.Text.RegularExpressions;

namespace BlazeGate.RBAC.Components.Extensions.AppCultureStorage
{
    public class AppCultureCookieStorageServices : IAppCultureStorageService
    {
        private readonly ICookieService cookieService;

        public AppCultureCookieStorageServices(ICookieService cookieService)
        {
            this.cookieService = cookieService;
        }

        public async Task<AppCultureInfo> GetAppCulture()
        {
            var result = await cookieService.GetCookieAsync<string>(CookieRequestCultureProvider.DefaultCookieName);
            var requestCulture = CookieRequestCultureProvider.ParseCookieValue(result);

            if (requestCulture?.Cultures != null)
            {
                AppCultureInfo appCultureInfo = new AppCultureInfo();
                appCultureInfo.Culture = requestCulture.Cultures.Count > 0 ? requestCulture.Cultures[0].Value : null;
                appCultureInfo.UICulture = requestCulture.UICultures.Count > 0 ? requestCulture.UICultures[0].Value : null;
                return appCultureInfo;
            }

            return null;
        }

        public async Task SetAppCulture(AppCultureInfo appCultureInfo)
        {
            if (appCultureInfo == null)
            {
                await cookieService.SetCookieAsync(CookieRequestCultureProvider.DefaultCookieName, string.Empty, TimeSpan.FromDays(365));
                return;
            }

            var culture = appCultureInfo?.Culture ?? "en-US";
            var uiCulture = appCultureInfo?.UICulture ?? "en-US";
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, uiCulture));
            await cookieService.SetCookieAsync(CookieRequestCultureProvider.DefaultCookieName, cookieValue, TimeSpan.FromDays(365));
        }
    }
}