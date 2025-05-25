using System.Text.Json;

namespace BlazeGate.RBAC.Components.Models
{
    public class PageNode : Model.EFCore.Page
    {
        public List<PageNode> Children { get; set; }

        public string ParentPageIdStr
        {
            get
            {
                return ParentPageId.ToString();
            }
            set
            {
                ParentPageId = long.Parse(string.IsNullOrWhiteSpace(value) ? "0" : value);
            }
        }

        public static List<PageNode> PageToPageNode(List<Model.EFCore.Page> totalPage, long? parentId)
        {
            List<PageNode> pageNodes = null;

            if (totalPage == null || totalPage.Count <= 0)
            {
                return pageNodes;
            }

            if (parentId == null)
            {
                parentId = totalPage.Select(b => b.ParentPageId).Min();
            }

            List<Model.EFCore.Page> pages = totalPage.Where(x => x.ParentPageId == parentId).OrderBy(x => x.IndexNumber).ToList();
            if (pages != null && pages.Count > 0)
            {
                pageNodes = new List<PageNode>();
                foreach (Model.EFCore.Page item in pages)
                {
                    //通过Page返序列化成PageNode
                    PageNode pageNode = JsonSerializer.Deserialize<PageNode>(JsonSerializer.Serialize(item));
                    pageNode.Children = PageToPageNode(totalPage, item.Id);
                    pageNodes.Add(pageNode);
                }
            }

            return pageNodes;
        }
    }
}