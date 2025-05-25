using AntDesign;
using Autofac.Core;
using BlazeGate.Common;
using BlazeGate.Dashboard.Components.Share;
using BlazeGate.Dashboard.Services;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Implement;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BlazeGate.Dashboard.Components.Pages.Service
{
    public partial class ServiceConfigIndex
    {
        [Parameter]
        public string ServiceName { get; set; }

        [Parameter]
        public EventCallback<long> OnFinish { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }

        [Inject]
        private IMessageService Message { get; set; }

        [Inject]
        public IBlazeGateServices BlazeGateServices { get; set; }

        public CopyToClipboard CopyToClipboard { get; set; }

        private IServiceService ServiceService { get; set; }

        private ServiceDetails ServiceDetails { get; set; } = new ServiceDetails();

        private bool loading = false;

        protected override async Task OnInitializedAsync()
        {
            ServiceService = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IServiceService>();

            await GetServiceDetailsByName(ServiceName);
        }

        public async Task GetServiceDetailsByName(string serviceName)
        {
            try
            {
                var result = await ServiceService.GetServiceDetailsByName(serviceName);
                if (result.Success)
                {
                    ServiceDetails = result.Data;
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error("加载异常:" + ex.Message);
            }
        }

        public async Task Save(EditContext editContext)
        {
            if (loading) return;
            loading = true;
            try
            {
                //更新数据库配置
                var result = await ServiceService.UpdateServiceDetails(ServiceDetails);
                if (result.Success)
                {
                    Message.Success(result.Msg);

                    //通知BlazeGate更新配置
                    AuthBaseInfo auth = new AuthBaseInfo();
                    auth.ServiceName = ServiceDetails.Service.ServiceName;
                    auth.Token = ServiceDetails.Service.Token;

                    var updateResult = await BlazeGateServices.YarpConfig_UpdateAll(auth);
                    if (!updateResult.Success)
                    {
                        Message.Error(updateResult.Msg);
                    }

                    if (OnFinish.HasDelegate)
                    {
                        await OnFinish.InvokeAsync(ServiceDetails.Service.Id);
                    }
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error("保存异常:" + ex.Message);
            }
            finally
            {
                loading = false;
            }
        }

        public async Task CopyDestinationConfig()
        {
            if (loading) return;
            loading = true;
            try
            {
                var result = await ServiceService.GetDestinationConfig(ServiceDetails.Service.Id);
                if (result.Success)
                {
                    if (await CopyToClipboard.Copy(result.Data))
                    {
                        Message.Success("复制成功");
                    }
                    else
                    {
                        Message.Error("复制失败");
                    }
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error("复制异常:" + ex.Message);
            }
            finally
            {
                loading = false;
            }
        }
    }
}