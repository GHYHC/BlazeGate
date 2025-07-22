using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Services.Interface;
using Microsoft.Extensions.Configuration;

namespace BlazeGate.Services.Implement.Remote
{
    public class PageService : AuthWebApi, IPageService
    {
        public PageService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            WebApiAddress = BlazeGateAddress;
        }

        public async Task<ApiResult<List<Page>>> GetPageByServiceName(string serviceName)
        {
            return await HttpPostJsonAsync<string, ApiResult<List<Page>>>($"/api/Page/GetPageByServiceName?serviceName={serviceName}", "");
        }

        public async Task<ApiResult<List<Page>>> GetUserPageByServiceName(string serviceName, long userId)
        {
            return await HttpPostJsonAsync<string, ApiResult<List<Page>>>($"/api/Page/GetUserPageByServiceName?serviceName={serviceName}&userId={userId}", "");
        }

        public async Task<ApiResult<int>> RemovePage(string serviceName, long pageId)
        {
            return await HttpPostJsonAsync<string, ApiResult<int>>($"/api/Page/RemovePage?serviceName={serviceName}&pageId={pageId}", "");
        }

        public Task<ApiResult<int>> SaveDrop(string serviceName, PageDropSave pageDropSave)
        {
            return HttpPostJsonAsync<PageDropSave, ApiResult<int>>($"/api/Page/SaveDrop?serviceName={serviceName}", pageDropSave);
        }

        public Task<ApiResult<long>> SavePage(string serviceName, Page page)
        {
            return HttpPostJsonAsync<Page, ApiResult<long>>($"/api/Page/SavePage?serviceName={serviceName}", page);
        }
    }
}