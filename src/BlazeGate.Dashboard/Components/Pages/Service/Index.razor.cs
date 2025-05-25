using AntDesign;
using BlazeGate.Dashboard.Services;
using BlazeGate.Common;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static BlazeGate.Dashboard.Components.Pages.ServiceList;

namespace BlazeGate.Dashboard.Components.Pages.Service
{
    public partial class Index : IDisposable
    {
        [Inject]
        private IMessageService Message { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }

        [Inject]
        public IBlazeGateServices BlazeGateServices { get; set; }

        private IServiceService ServiceService { get; set; }

        private ServiceList ServiceList { get; set; }

        private List<HealthStateCount> HealthStateCounts { get; set; } = new List<HealthStateCount>();

        private System.Threading.Timer timer { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ServiceService = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IServiceService>();

            //定时刷新健康状态
            await TimerStart();
        }

        private async Task AddService()
        {
            try
            {
                var result = await ServiceService.AddService();
                if (result.Success)
                {
                    Message.Success("添加成功");
                    await ServiceList.LoadData();
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"添加异常:{ex.Message}");
            }
        }

        public async Task DeleteService(long serviceId)
        {
            try
            {
                var result = await ServiceService.DeleteService(serviceId);
                if (result.Success)
                {
                    Message.Success("删除成功");
                    await ServiceList.LoadData();
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"删除异常:{ex.Message}");
            }
        }

        private async Task EnabledChanged(long serviceId, bool b)
        {
            try
            {
                var result = await ServiceService.EnabledChanged(serviceId, b);
                if (result.Success)
                {
                    var service = ServiceList.Services.FirstOrDefault(b => b.Id == serviceId);

                    //通知BlazeGate更新配置
                    AuthBaseInfo auth = new AuthBaseInfo();
                    auth.ServiceName = service.ServiceName;
                    auth.Token = service.Token;

                    var updateResult = await BlazeGateServices.YarpConfig_UpdateAll(auth);
                    if (!updateResult.Success)
                    {
                        Message.Error(updateResult.Msg);
                    }

                    await ServiceList.Refresh(serviceId);
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"修改异常:{ex.Message}");
            }
        }

        private async Task GetHealthStateCounts()
        {
            try
            {
                var service = ServiceList?.Services?.FirstOrDefault();
                if (service == null)
                {
                    return;
                }

                AuthBaseInfo auth = new AuthBaseInfo();
                auth.ServiceName = service.ServiceName;
                auth.Token = service.Token;

                var result = await BlazeGateServices.Destination_GetHealthStateCounts(auth);

                if (result.Success)
                {
                    HealthStateCounts = result.Data;
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"获取异常:{ex.Message}");
            }
        }

        /// <summary>
        /// 自动刷新
        /// </summary>
        public async Task TimerStart()
        {
            //定时器
            timer = new System.Threading.Timer(async (args) =>
            {
                await GetHealthStateCounts();
                // 通知Blazor更新UI
                await InvokeAsync(() => { StateHasChanged(); });
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));//5秒执行一次
        }

        public void Dispose()
        {
            // 组件被销毁时停止定时器
            timer?.Dispose();
        }
    }
}