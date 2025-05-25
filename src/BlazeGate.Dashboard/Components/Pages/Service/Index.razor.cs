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

            //��ʱˢ�½���״̬
            await TimerStart();
        }

        private async Task AddService()
        {
            try
            {
                var result = await ServiceService.AddService();
                if (result.Success)
                {
                    Message.Success("��ӳɹ�");
                    await ServiceList.LoadData();
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"����쳣:{ex.Message}");
            }
        }

        public async Task DeleteService(long serviceId)
        {
            try
            {
                var result = await ServiceService.DeleteService(serviceId);
                if (result.Success)
                {
                    Message.Success("ɾ���ɹ�");
                    await ServiceList.LoadData();
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"ɾ���쳣:{ex.Message}");
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

                    //֪ͨBlazeGate��������
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
                Message.Error($"�޸��쳣:{ex.Message}");
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
                Message.Error($"��ȡ�쳣:{ex.Message}");
            }
        }

        /// <summary>
        /// �Զ�ˢ��
        /// </summary>
        public async Task TimerStart()
        {
            //��ʱ��
            timer = new System.Threading.Timer(async (args) =>
            {
                await GetHealthStateCounts();
                // ֪ͨBlazor����UI
                await InvokeAsync(() => { StateHasChanged(); });
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));//5��ִ��һ��
        }

        public void Dispose()
        {
            // ���������ʱֹͣ��ʱ��
            timer?.Dispose();
        }
    }
}