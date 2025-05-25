using BlazeGate.JwtBearer;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Services.Interface;
using BlazeGate.Authorization;
using BlazeGate.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazeGate.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PageController : Controller
    {
        private readonly BlazeGateContext BlazeGateContext;
        private readonly IPageService pageService;

        public PageController(BlazeGateContext BlazeGateContext, IPageService pageService)
        {
            this.BlazeGateContext = BlazeGateContext;
            this.pageService = pageService;
        }

        [HttpPost]
        [ServiceFilter(typeof(RBACAuthFilter))]
        public async Task<ApiResult<List<Page>>> GetPageByServiceName(string serviceName)
        {
            return await pageService.GetPageByServiceName(serviceName);
        }

        [HttpPost]
        [ServiceFilter(typeof(RBACAuthFilter))]
        public async Task<ApiResult<long>> SavePage(string serviceName, Page page)
        {
            return await pageService.SavePage(serviceName, page);
        }

        [HttpPost]
        [ServiceFilter(typeof(RBACAuthFilter))]
        public async Task<ApiResult<int>> RemovePage(string serviceName, long pageId)
        {
            return await pageService.RemovePage(serviceName, pageId);
        }

        [HttpPost]
        [ServiceFilter(typeof(RBACAuthFilter))]
        public async Task<ApiResult<int>> SaveDrop(string serviceName, PageDropSave pageDropSave)
        {
            return await pageService.SaveDrop(serviceName, pageDropSave);
        }

        [HttpPost]
        public async Task<ApiResult<List<Page>>> GetUserPageByServiceName(string serviceName)
        {
            UserDto user = User.GetUser();
            return await pageService.GetUserPageByServiceName(serviceName, user.Id);
        }
    }
}