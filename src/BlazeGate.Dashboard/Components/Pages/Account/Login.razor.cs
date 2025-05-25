using AntDesign;
using BlazeGate.Dashboard.Components.Share;
using BlazeGate.Dashboard.Models;
using Microsoft.AspNetCore.Components;
using static BlazeGate.Dashboard.Components.Share.Ajax;

namespace BlazeGate.Dashboard.Components.Pages.Account
{
    public partial class Login
    {
        private readonly LoginParams _model = new LoginParams();

        private bool _loading = false;

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public MessageService Message { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "returnUrl")]
        public string ReturnUrl { get; set; } = "/";

        private Ajax Ajax { get; set; }

        protected override Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public async Task HandleSubmit()
        {
            if (_loading)
            {
                return;
            }

            _loading = true;
            try
            {
                var ajaxOption = new AjaxOption
                {
                    Url = "/api/account/login",
                    Data = _model
                };

                var result = await Ajax.Send<ApiResult<string>>(ajaxOption);
                if (result.Code == 1)
                {
                    NavigationManager.NavigateTo(ReturnUrl ?? "/", true);
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error("登录异常");
            }
            finally
            {
                _loading = false;
            }
        }
    }
}