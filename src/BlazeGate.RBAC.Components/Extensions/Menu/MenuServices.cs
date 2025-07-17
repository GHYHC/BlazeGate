using AntDesign.ProLayout;
using BlazeGate.Model.EFCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BlazeGate.RBAC.Components.Extensions.Menu
{
    public class MenuServices : IMenuServices
    {
        private readonly ILogger<MenuServices> logger;
        private readonly IPageDataStorageServices pageDataStorageServices;

        public MenuServices(ILogger<MenuServices> logger, IPageDataStorageServices pageDataStorageServices)
        {
            this.logger = logger;
            this.pageDataStorageServices = pageDataStorageServices;
        }

        public async Task<List<MenuDataItem>> GetMenu(IStringLocalizer stringLocalizer = null)
        {
            List<MenuDataItem> menus = new List<MenuDataItem>();

            try
            {
                var pages = await pageDataStorageServices.GetPageData();
                var menuList = ConvertMenu(pages, null, stringLocalizer);
                if (menuList != null && menuList.Count > 0)
                {
                    menus = menuList;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取菜单异常");
            }

            return menus;
        }

        private List<MenuDataItem> ConvertMenu(List<Page> totalPage, long? parentId, IStringLocalizer stringLocalizer = null)
        {
            List<MenuDataItem> menus = null;

            if (totalPage == null || totalPage.Count <= 0)
            {
                return menus;
            }

            if (parentId == null)
            {
                parentId = totalPage.Where(b => b.Type == 0).Select(b => b.ParentPageId).Min();
            }

            List<Page> pages = totalPage.Where(x => x.ParentPageId == parentId && x.Type == 0).OrderBy(x => x.IndexNumber).ToList();
            if (pages != null && pages.Count > 0)
            {
                menus = new List<MenuDataItem>();
                foreach (Page item in pages)
                {
                    MenuDataItem menu = new MenuDataItem();
                    menu.Name = stringLocalizer == null ? item.Title : stringLocalizer[item.Title];
                    menu.Path = item.Path;
                    menu.Key = item.Id.ToString();
                    menu.Icon = item.Icon;
                    menu.Children = ConvertMenu(totalPage, item.Id)?.ToArray();
                    menus.Add(menu);
                }
            }

            return menus;
        }
    }

    public interface IMenuServices
    {
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        Task<List<MenuDataItem>> GetMenu(IStringLocalizer stringLocalizer = null);
    }
}