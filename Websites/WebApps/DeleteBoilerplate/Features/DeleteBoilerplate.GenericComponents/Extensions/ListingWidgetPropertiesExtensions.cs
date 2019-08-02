using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CMS.DocumentEngine;
using DeleteBoilerplate.GenericComponents.Models.Widgets;

namespace DeleteBoilerplate.GenericComponents.Extensions
{
    public static class ListingWidgetPropertiesExtensions
    {
        public static List<TTreeNode> GetPages<TTreeNode>(this IListingWidgetProperties<TTreeNode> listingProperties) where TTreeNode : TreeNode, new()
        {
            var selectedPagePath = listingProperties.Items?.FirstOrDefault()?.NodeAliasPath;

            if (!string.IsNullOrWhiteSpace(selectedPagePath))
            {
                var className = typeof(TTreeNode).GetField("CLASS_NAME", BindingFlags.Public | BindingFlags.Static)?.GetRawConstantValue();

                if (className != null)
                {
                    return DocumentHelper.GetDocuments<TTreeNode>()
                        .WhereEquals("ClassName", className)
                        .And()
                        .WhereStartsWith("NodeAliasPath", selectedPagePath)
                        .ToList();
                }
            }

            return new List<TTreeNode>();
        }
    }
}