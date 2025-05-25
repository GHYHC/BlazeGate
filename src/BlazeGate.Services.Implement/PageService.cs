using BlazeGate.Common.Autofac;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Implement
{
    public class PageService : IPageService, IScopeDenpendency
    {
        private readonly BlazeGateContext BlazeGateContext;
        private readonly ISnowFlakeService snowFlake;

        public PageService(BlazeGateContext BlazeGateContext, ISnowFlakeService snowFlake)
        {
            this.BlazeGateContext = BlazeGateContext;
            this.snowFlake = snowFlake;
        }

        public async Task<ApiResult<List<Page>>> GetPageByServiceName(string serviceName)
        {
            var list = await BlazeGateContext.Pages.AsNoTracking().Where(p => p.ServiceName == serviceName).ToListAsync();
            return ApiResult<List<Page>>.SuccessResult(list);
        }

        public async Task<ApiResult<int>> RemovePage(string serviceName, long pageId)
        {
            //删除页面以及子页面
            List<long> removePageIds = GetChildPageId(pageId);
            removePageIds.Add(pageId);

            var i = await BlazeGateContext.Pages.Where(p => p.ServiceName == serviceName && removePageIds.Contains(p.Id)).ExecuteDeleteAsync();
            return ApiResult<int>.SuccessResult(i);
        }

        private List<long> GetChildPageId(long pageId)
        {
            List<long> pageIdList = BlazeGateContext.Pages.AsNoTracking().Where(p => p.ParentPageId == pageId).Select(p => p.Id).ToList();
            if (pageIdList.Count > 0)
            {
                foreach (var item in pageIdList)
                {
                    pageIdList.AddRange(GetChildPageId(item));
                }
            }
            return pageIdList;
        }

        public async Task<ApiResult<long>> SavePage(string serviceName, Page page)
        {
            page.ServiceId = await BlazeGateContext.Services.Where(b => b.ServiceName == page.ServiceName).Select(b => b.Id).FirstOrDefaultAsync();
            if (page.Id == 0)
            {
                page.Id = await snowFlake.NextId();
                page.CreateTime = DateTime.Now;
                page.UpdateTime = DateTime.Now;
                BlazeGateContext.Pages.Add(page);
            }
            else
            {
                page.UpdateTime = DateTime.Now;
                BlazeGateContext.Pages.Update(page);
            }
            int i = await BlazeGateContext.SaveChangesAsync();
            return ApiResult<long>.Result(i > 0, page.Id);
        }

        public async Task<ApiResult<int>> SaveDrop(string serviceName, PageDropSave pageDropSave)
        {
            var pages = await BlazeGateContext.Pages.Where(p => pageDropSave.SortIds.Contains(p.Id)).ToListAsync();

            for (int i = 0; i < pageDropSave.SortIds.Count; i++)
            {
                var page = pages.FirstOrDefault(b => b.Id == pageDropSave.SortIds[i]);
                if (page == null)
                {
                    break;
                }

                page.IndexNumber = i;
                if (page.Id == pageDropSave.Page.Id)
                {
                    page.ParentPageId = pageDropSave.Page.ParentPageId;
                }
            }

            int result = await BlazeGateContext.SaveChangesAsync();
            return ApiResult<int>.SuccessResult(result);
        }

        public async Task<ApiResult<List<Page>>> GetUserPageByServiceName(string serviceName, long userId)
        {
            long serviceId = await BlazeGateContext.Services.Where(b => b.ServiceName == serviceName && b.Enabled == true).Select(b => b.Id).FirstOrDefaultAsync();
            if (serviceId <= 0)
            {
                return ApiResult<List<Page>>.FailResult("服务不存在");
            }

            //查询出服务对应的角色
            var roleIds = await BlazeGateContext.UserRoles.Where(b => b.ServiceId == serviceId && b.UserId == userId).Select(b => b.RoleId).ToListAsync();

            //查询出角色对应的页面
            var pageIds = await BlazeGateContext.RolePages.Where(b => roleIds.Contains(b.RoleId) && b.ServiceId == serviceId).Select(b => b.PageId).ToListAsync();

            //查询出页面
            var pages = await BlazeGateContext.Pages.Where(b => pageIds.Contains(b.Id) && b.ServiceId == serviceId).ToListAsync();

            return ApiResult<List<Page>>.SuccessResult(pages);
        }
    }
}