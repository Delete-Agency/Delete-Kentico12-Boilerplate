using System.Collections.Generic;
using CMS.DocumentEngine;
using Kentico.Components.Web.Mvc.FormComponents;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets
{
    public interface IListingWidgetProperties<TTreeNode> where TTreeNode : TreeNode
    {
        List<PathSelectorItem> Items { get; set; }
    }
}