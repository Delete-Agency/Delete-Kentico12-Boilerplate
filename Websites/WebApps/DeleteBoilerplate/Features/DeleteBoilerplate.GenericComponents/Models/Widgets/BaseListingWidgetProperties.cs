using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine;
using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets
{
    public abstract class BaseListingWidgetProperties<TTreeNode> : BaseWidgetViewModel, IWidgetProperties 
        where TTreeNode : TreeNode, new()
    {
        [EditingComponent(PathSelector.IDENTIFIER)]
        [EditingComponentProperty(nameof(PathSelectorProperties.RootPath), "/")]
        [EditingComponentProperty(nameof(PathSelectorProperties.Label), "Listing Root")]
        public List<PathSelectorItem> Items { get; set; }

        public virtual List<TTreeNode> GetPages()
        {
            var selectedPagePath = Items?.FirstOrDefault()?.NodeAliasPath;

            if (!string.IsNullOrWhiteSpace(selectedPagePath))
            {
                return DocumentHelper.GetDocuments<TTreeNode>()
                    .WhereEquals("ClassName", new TTreeNode().ClassName)
                    .And()
                    .WhereStartsWith("NodeAliasPath", selectedPagePath)
                    .ToList();
            }

            return new List<TTreeNode>();
        }
    }
}