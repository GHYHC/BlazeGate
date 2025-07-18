// <auto-generated/>
using AntDesign;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.RBAC.Components.Models;
using BlazeGate.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;

namespace BlazeGate.RBAC.Components.Pages.Page
{
    public partial class PageIndex
    {
        [Parameter]
        public string ServiceName { get; set; }

        private bool Loading { get; set; } = false;

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }

        [Inject]
        private IConfiguration Configuration { get; set; }

        [Inject]
        private IPageService PageService { get; set; }

        private List<PageNode> TreeList { get; set; } = new List<PageNode>();

        private List<Model.EFCore.Page> PageList { get; set; } = new List<Model.EFCore.Page>();

        private Tree<PageNode> Tree { get; set; }

        private PageNode Model { get; set; } = new PageNode();
        private Form<PageNode> Form { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrWhiteSpace(ServiceName))
            {
                ServiceName = Configuration["BlazeGate:ServiceName"];
            }
            await LoadTreeList(ServiceName);
        }

        private async Task LoadTreeList(string serviceName)
        {
            if (!(PageService is BlazeGate.Services.Implement.Remote.PageService))
            {
                PageService = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IPageService>();
            }

            if (Loading) return;
            Loading = true;
            try
            {
                var result = await PageService.GetPageByServiceName(serviceName);
                if (result.Success)
                {
                    PageList = result.Data;
                    TreeList = PageNode.PageToPageNode(result.Data, null);
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error(string.Format(L["page.get.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }
        }

        private async Task SelectedKeysChanged(string[] keys)
        {
            if (keys.Length > 0)
            {
                var key = keys[0];
                var pageNode = await GetPageNodeByKey(key, TreeList);
                if (pageNode != null)
                {
                    Model = JsonSerializer.Deserialize<PageNode>(JsonSerializer.Serialize(pageNode));
                }
            }
        }

        public async Task<PageNode> GetPageNodeByKey(string key, List<PageNode> pageNodes)
        {
            var pageNode = pageNodes.FirstOrDefault(b => b.Id.ToString() == key);
            if (pageNode == null)
            {
                foreach (var item in pageNodes)
                {
                    if (item.Children != null && item.Children.Count > 0)
                    {
                        pageNode = await GetPageNodeByKey(key, item.Children);
                        if (pageNode != null)
                        {
                            return pageNode;
                        }
                    }
                }
            }
            return pageNode;
        }

        private async Task OnFinish(EditContext editContext)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                Model.ServiceName = ServiceName;
                Model.Icon = Model.Icon ?? "";
                Model.Title = Model.Title ?? "";
                Model.Path = Model.Path ?? "";
                Model.SubPath = Model.SubPath ?? "";

                if (Model.Id == 0)
                {
                    Model.IndexNumber = PageList.Where(b => b.ParentPageId == Model.ParentPageId).Select(b => b.IndexNumber).DefaultIfEmpty(-1).Max() + 1;
                }

                var result = await PageService.SavePage(ServiceName, Model);
                if (result.Success)
                {
                    Model.Id = result.Data;
                    Message.Success(result.Msg);
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error(string.Format(L["page.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }

            //重新加载树
            await LoadTreeList(ServiceName);

            //重新选中节点
            Tree.SelectedKeys = new string[] { Model.Id.ToString() };
            await Tree.SelectedKeysChanged.InvokeAsync(Tree.SelectedKeys);
        }

        private async Task OnRemove(long pageId)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                var result = await PageService.RemovePage(ServiceName, pageId);
                if (result.Success)
                {
                    Model = new PageNode();
                    Message.Success(result.Msg);
                }
                else
                {
                    Message.Error(result.Msg);
                }
            }
            catch (Exception ex)
            {
                Message.Error(string.Format(L["page.delete.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }

            await LoadTreeList(ServiceName);
        }

        private async Task OnDrop(TreeEventArgs<PageNode> treeEventArgs)
        {
            if (Loading) return;
            Loading = true;
            try
            {
                PageNode target = treeEventArgs.TargetNode.DataItem;
                PageNode node = treeEventArgs.Node.DataItem;

                if (node.Id == target.Id)
                {
                    Message.Error(L["page.drag.error"].Value);
                    return;
                }

                List<PageNode> pageNodes = new List<PageNode>();
                if (!treeEventArgs.DropBelow)
                {
                    node.ParentPageId = target.Id;
                    if (treeEventArgs.TargetNode.DataItem.Children == null)
                    {
                        pageNodes.Add(node);
                    }
                    else
                    {
                        foreach (var children in treeEventArgs.TargetNode.DataItem.Children)
                        {
                            if (children.Id == node.Id)
                            {
                                pageNodes.Insert(0, children);
                            }
                            else
                            {
                                pageNodes.Add(children);
                            }
                        }
                    }
                }
                else
                {
                    node.ParentPageId = target.ParentPageId;
                    pageNodes = treeEventArgs.TargetNode.GetSiblingNodes().Select(b => b.DataItem).ToList();

                    //删除当前节点，并插入到目标节点的后面
                    var b = pageNodes.Remove(node);
                    pageNodes.Insert(pageNodes.FindIndex(b => b.Id == target.Id) + 1, node);
                }

                PageDropSave pageDropSave = new PageDropSave()
                {
                    Page = node,
                    SortIds = pageNodes.Select(b => b.Id).ToList()
                };

                var result = await PageService.SaveDrop(ServiceName, pageDropSave);
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
                Message.Error(string.Format(L["page.error"], ex.Message));
            }
            finally
            {
                Loading = false;
            }

            await LoadTreeList(ServiceName);

            //重新选中节点
            if (treeEventArgs.Tree.SelectedKeys != null)
            {
                await treeEventArgs.Tree.SelectedKeysChanged.InvokeAsync(treeEventArgs.Tree.SelectedKeys);
            }
        }
    }
}