using AntDesign;
using BlazeGate.Model.EFCore;
using BlazeGate.Services.Implement;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Components;

namespace BlazeGate.Dashboard.Components.Pages.Service
{
    public partial class AuthWhiteListIndex
    {
        [Parameter]
        public string ServiceName { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }

        [Inject]
        private IMessageService Message { get; set; }

        private IAuthWhiteListService authWhiteListService { get; set; }

        public string AuthWhiteListStr { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            authWhiteListService = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IAuthWhiteListService>();
            await GetWhiteListByServiceName(ServiceName);
        }

        public async Task GetWhiteListByServiceName(string serviceName)
        {
            try
            {
                var result = await authWhiteListService.GetWhiteListByServiceName(serviceName);
                if (result.Success)
                {
                    AuthWhiteListStr = string.Join("\r\n", result.Data?.Select(b => b.Address));
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error(string.Format(L["authWhiteList.load.error"], ex.Message));
            }
        }

        public async Task Save()
        {
            try
            {
                //整理白名单
                List<AuthWhiteList> authWhiteLists = new List<AuthWhiteList>();
                if (!string.IsNullOrWhiteSpace(AuthWhiteListStr))
                {
                    var authWhiteListStrs = AuthWhiteListStr.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var address in authWhiteListStrs)
                    {
                        if (!string.IsNullOrWhiteSpace(address.Trim()))
                        {
                            AuthWhiteList authWhiteList = new AuthWhiteList();
                            authWhiteList.Address = address.Trim();
                            authWhiteLists.Add(authWhiteList);
                        }
                    }
                }

                var result = await authWhiteListService.SaveWhiteList(ServiceName, authWhiteLists);
                if (result.Success)
                {
                    Message.Success(L["authWhiteList.save.success"].Value);
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error(string.Format(L["authWhiteList.save.error"], ex.Message));
            }
        }
    }
}