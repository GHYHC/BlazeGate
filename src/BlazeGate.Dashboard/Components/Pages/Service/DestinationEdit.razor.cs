using AntDesign;
using BlazeGate.Dashboard.Services;
using BlazeGate.Common;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.RBAC.Components;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;

namespace BlazeGate.Dashboard.Components.Pages.Service
{
    public partial class DestinationEdit
    {
        [Parameter]
        public EventCallback OnComplete { get; set; }

        [Inject]
        public MessageService Message { get; set; }

        private string Title { get; set; } = "ÐÂÔö";

        private bool Visible { get; set; } = false;

        private bool Loading { get; set; } = false;

        private Form<DestinationInfo> Form { get; set; }

        private DestinationInfo DestinationInfo { get; set; } = new DestinationInfo();

        [Inject]
        public IBlazeGateServices BlazeGateServices { get; set; }

        protected override async Task OnInitializedAsync()
        {
        }

        public async Task ShowAsync(string address, string serviceName, string serviceToken)
        {
            DestinationInfo.Address = address;
            DestinationInfo.ServiceName = serviceName;
            DestinationInfo.Token = serviceToken;
            Visible = true;
        }

        private async Task OnFinish(EditContext editContext)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                var result = await BlazeGateServices.Destination_Add(DestinationInfo);
                if (result.Success)
                {
                    Message.Success(result.Msg);
                    Visible = false;
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"Òì³£:{ex.Message}");
            }
            finally
            {
                Loading = false;
            }

            if (OnComplete.HasDelegate)
            {
                await OnComplete.InvokeAsync();
            }
        }
    }
}