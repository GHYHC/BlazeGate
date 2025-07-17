using AntDesign.Extensions.Localization;
using AntDesign.ProLayout;
using BlazeGate.RBAC.Components.Extensions.Menu;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;
using System.Net.Http.Json;

namespace BlazeGate.BlazorWebApp.Sample.Layouts
{
    public partial class BasicLayout : LayoutComponentBase, IDisposable
    {
        private MenuDataItem[] MenuData;

        [Inject]
        private ReuseTabsService TabService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Inject]
        private IMenuServices MenuServices { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //关闭之前的所有页面
            TabService.CloseAll();

            //验证是否登录
            var authStae = await authenticationState;
            var user = authStae.User;
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var menuList = await MenuServices.GetMenu();
                MenuData = menuList.ToArray();
            }
        }

        private void Reload()
        {
            TabService.ReloadPage();
        }

        public void Dispose()
        {
        }
    }
}