using BlazeGate.Model.EFCore;
using BlazeGate.Model.WebApi;
using BlazeGate.Model.WebApi.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Services.Interface
{
    public interface IPageService
    {
        /// <summary>
        /// 根据服务名称获取页面
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public Task<ApiResult<List<Page>>> GetPageByServiceName(string serviceName);

        /// <summary>
        /// 保存页面
        /// </summary>
        /// <param name="roleSave"></param>
        /// <returns></returns>
        public Task<ApiResult<long>> SavePage(string serviceName, Page page);

        /// <summary>
        /// 删除页面
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public Task<ApiResult<int>> RemovePage(string serviceName, long pageId);

        /// <summary>
        /// 保存页面拖拽
        /// </summary>
        /// <param name="pageDropSave"></param>
        /// <returns></returns>
        public Task<ApiResult<int>> SaveDrop(string serviceName, PageDropSave pageDropSave);

        /// <summary>
        /// 根据服务名称获取用户页面
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<ApiResult<List<Page>>> GetUserPageByServiceName(string serviceName, long userId);
    }
}