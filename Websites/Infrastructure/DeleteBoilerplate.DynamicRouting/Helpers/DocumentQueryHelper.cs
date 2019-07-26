using System.Linq;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;

namespace DeleteBoilerplate.DynamicRouting.Helpers
{
    public class DocumentQueryHelper
    {
        /// <summary>
        /// Gets the query to get TreeNode for the corresponding path
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static MultiDocumentQuery GetNodeByAliasPathOrSeoUrlQuery(string url)
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

            // Specific page query
            var query = DocumentHelper.GetDocuments()
                .Types(pageTypesWithSeoUrlClassNames)
                .Columns("NodeAliasPath", "SeoUrl", "DocumentID")
                .WhereEquals("SeoUrl", url)
                .Or()
                .WhereEquals("NodeAliasPath", url)
                .TopN(1);

            return query;
        }
    }
}