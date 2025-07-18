// <auto-generated/>
using AntDesign;
using AntDesign.TableModels;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using BlazeGate.RBAC.Components.Resources;

namespace BlazeGate.RBAC.Components.Pages.User
{
    public partial class UserIndex
    {
        [Inject]
        private IServiceProvider ServiceProvider { get; set; }
        [Inject]
        private IConfiguration Configuration { get; set; }
        [Inject]
        private IUserService UserService { get; set; }
        private string NameOrAccount { get; set; }
        private List<UserInfo> DataList { get; set; } = new List<UserInfo>();
        UserQuery SearchModel { get; set; } = new UserQuery();
        private Table<UserInfo> Table { get; set; }
        private bool Loading { get; set; } = false;
        private int Total { get; set; } = 0;
        private UserEdit UserEdit { get; set; }

        public string ServiceName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ServiceName = Configuration["BlazeGate:ServiceName"];

            if (!(UserService is BlazeGate.Services.Implement.Remote.UserService))
            {
                UserService = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IUserService>();
            }
        }

        private async Task OnChange(QueryModel<UserInfo> queryModel)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                var result = await UserService.QueryByPage(ServiceName, queryModel.PageIndex, queryModel.PageSize, SearchModel);

                if (result.Success)
                {
                    DataList = result.Data.DataList;
                    Total = result.Data.Total;
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error(string.Format(L["user.get.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }
        }

        private async Task OnRemove(long id)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                var result = await UserService.RemoveById(ServiceName,id);
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
                Message.Error(string.Format(L["user.delete.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }
        }

        private async Task OnChangeEnabled(long id, bool enabled)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                var result = await UserService.ChangeUserEnabled(ServiceName,id, enabled);
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
                Message.Error(string.Format(L["user.status.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }
        }
    }
}