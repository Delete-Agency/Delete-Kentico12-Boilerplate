using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine;
using DeleteBoilerplate.GenericComponents.Models.Widgets;
using Newtonsoft.Json;

namespace DeleteBoilerplate.GenericComponents.Extensions
{
    public static class WidgetPropertiesExtensions
    {
        public static string GetRootPageAliasPath<TTreeNode>(this IListingWidgetProperties<TTreeNode> listingProperties)
            where TTreeNode : TreeNode, new()
        {
            return listingProperties.Items?.FirstOrDefault()?.NodeAliasPath;
        }

        public static List<TTreeNode> GetPages<TTreeNode>(this IListingWidgetProperties<TTreeNode> listingProperties) where TTreeNode : TreeNode, new()
        {
            var selectedPagePath = listingProperties.GetRootPageAliasPath();

            if (!string.IsNullOrWhiteSpace(selectedPagePath))
            {
                return DocumentHelper.GetDocuments<TTreeNode>()
                    .WhereStartsWith("NodeAliasPath", selectedPagePath)
                    .ToList();
            }

            return new List<TTreeNode>();
        }

        public static List<string> ToTaxonomiesList(this ITaxonomyWidgetProperties properties)
        {
            if (string.IsNullOrWhiteSpace(properties.Type)) return new List<string>();
            return JsonConvert.DeserializeObject<IEnumerable<string>>(properties.Type).ToList();
        }
    }
}