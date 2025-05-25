using AntDesign;
using BlazeGate.Services.Implement;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Components;

namespace BlazeGate.Dashboard.Components.Pages
{
    public partial class ServiceList
    {
        private SearchModel Model { get; set; } = new SearchModel();

        [Inject]
        private IMessageService Message { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }

        private IServiceService ServiceService { get; set; }

        public List<Model.EFCore.Service> Services { get; set; } = new List<Model.EFCore.Service>();

        private List<string> ActiveKey { get; set; } = new List<string>();

        private Collapse Collapse { get; set; }

        [Parameter]
        public RenderFragment FunctionTemplate { get; set; }

        [Parameter]
        public RenderFragment<Model.EFCore.Service> HeaderTemplate { get; set; }

        [Parameter]
        public RenderFragment<Model.EFCore.Service> ChildContent { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ServiceService = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IServiceService>();
            await LoadData();
        }

        public async Task LoadData()
        {
            try
            {
                var result = await ServiceService.LoadData(Model.ServiceName);
                if (result.Success)
                {
                    Services = result.Data;
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"加载数据异常:{ex.Message}");
            }

            Collapse.Deactivate(ActiveKey.ToArray());
        }

        //刷新
        public async Task Refresh(long serviceId)
        {
            try
            {
                var result = await ServiceService.GetServiceById(serviceId);
                if (result.Success)
                {
                    var service = Services.FirstOrDefault(p => p.Id == serviceId);

                    //将新数据替换旧数据
                    if (service != null)
                    {
                        var index = Services.IndexOf(service);
                        Services[index] = result.Data;
                    }
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error($"刷新异常:{ex.Message}");
            }
        }

        public class SearchModel
        {
            public string ServiceName { get; set; }
        }
    }
}