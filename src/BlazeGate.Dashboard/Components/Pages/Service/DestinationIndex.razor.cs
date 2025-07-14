using AntDesign.TableModels;
using AntDesign;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Components;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Common;
using static BlazeGate.Dashboard.Components.Pages.ServiceList;
using BlazeGate.Dashboard.Services;

namespace BlazeGate.Dashboard.Components.Pages.Service
{
    public partial class DestinationIndex
    {
        [Parameter]
        public string ServiceName { get; set; }

        [Parameter]
        public string ServiceToken { get; set; }

        private bool Loading { get; set; } = false;
        private DestinationQurey SearchModel { get; set; } = new DestinationQurey();
        private Table<Destination> Table { get; set; }
        private int Total { get; set; } = 0;
        private List<Destination> DataList { get; set; } = new List<Destination>();

        private DestinationEdit DestinationEdit { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }

        [Inject]
        public IBlazeGateServices BlazeGateServices { get; set; }

        [Inject]
        private IMessageService Message { get; set; }

        protected override async Task OnInitializedAsync()
        {
        }

        private async Task OnChange(QueryModel<Destination> queryModel)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                SearchModel.ServiceName = ServiceName;
                SearchModel.Token = ServiceToken;
                var result = await BlazeGateServices.Destination_GetDestinations(SearchModel);

                if (result.Success)
                {
                    DataList = result.Data;
                    Total = result.Data.Count;
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error(string.Format(L["destination.index.get.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }
        }

        private async Task OnRemove(string address)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                DestinationInfo destinationInfo = new DestinationInfo();
                destinationInfo.ServiceName = ServiceName;
                destinationInfo.Token = ServiceToken;
                destinationInfo.Address = address;

                var result = await BlazeGateServices.Destination_Remove(destinationInfo);
                if (result.Success)
                {
                    Message.Success(result.Msg);
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error(string.Format(L["destination.index.delete.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }
        }

        private TextElementType GetTextTypeByHealthState(string healthState)
        {
            if (healthState == "Healthy")
            {
                return TextElementType.Success;
            }
            else if (healthState == "Unhealthy")
            {
                return TextElementType.Danger;
            }
            else if (healthState == "Unknown")
            {
                return TextElementType.Warning;
            }
            else
            {
                return TextElementType.Secondary;
            }
        }
    }
}