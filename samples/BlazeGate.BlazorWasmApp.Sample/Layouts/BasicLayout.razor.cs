using AntDesign.ProLayout;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazeGate.BlazorWasmApp.Sample.Layouts
{
    public partial class BasicLayout : LayoutComponentBase, IDisposable
    {
        private MenuDataItem[] MenuData { get; set; }

        [Inject] private ReuseTabsService TabService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
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