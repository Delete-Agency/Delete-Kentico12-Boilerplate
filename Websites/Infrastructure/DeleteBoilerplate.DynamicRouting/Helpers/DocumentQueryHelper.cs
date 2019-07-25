using System;
using System.Linq;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;

namespace DeleteBoilerplate.DynamicRouting.Helpers
{
    public class DocumentQueryHelper
    {
        /// <summary>
        /// Gets the TreeNode for the corresponding path, can be either the NodeAliasPath or a URL Alias
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static TreeNode GetNodeByAliasPathOrSeoUrl(string url)
        {
            // All page types
            var allPageTypes = DataClassInfoProvider.GetClasses()
                .Where(dataClass => dataClass.ClassIsDocumentType)
                .ToList();

            var basePageType = allPageTypes
                .FirstOrDefault(x => x.ClassName == BasePage.CLASS_NAME);

            // All page types with SeoUrl column
            var pageTypesWithSeoUrlClassNames = allPageTypes
                .Where(x => x.ClassInheritsFromClassID == basePageType?.ClassID)
                .Select(x => x.ClassName)
                .ToArray();

            // How to get value for specific page
            var treeNode = DocumentHelper.GetDocuments()
                .Types(pageTypesWithSeoUrlClassNames)
                .Columns("NodeAliasPath", "SeoUrl", "DocumentID")
                .WhereEquals("SeoUrl", url)
                .Or()
                .WhereEquals("NodeAliasPath", url)
                .TopN(1)
                .FirstOrDefault();

            return treeNode;
        }
    }
}