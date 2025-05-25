using BlazeGate.Model.EFCore;

namespace BlazeGate.RBAC.Components.Extensions.Menu
{
    /// <summary>
    /// 页面数据存储服务
    /// </summary>
    public class PageDataStorageServices : IPageDataStorageServices
    {
        private List<Page> pageData;

        public PageDataStorageServices()
        {
            pageData = new List<Page>();
        }

        public async Task<List<Page>> GetPageData()
        {
            return pageData;
        }

        public async Task SetPageData(List<Page> pageData)
        {
            this.pageData = pageData;
        }
    }

    public interface IPageDataStorageServices
    {
        Task<List<Page>> GetPageData();

        Task SetPageData(List<Page> pageData);
    }
}