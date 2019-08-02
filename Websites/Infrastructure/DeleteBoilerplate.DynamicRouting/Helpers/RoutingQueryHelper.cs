using System.Collections.Generic;
using System.Linq;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.DynamicRouting.Helpers
{
    public class RoutingQueryHelper
    {
        /// <summary>
        /// Gets the query returning TreeNode for the corresponding url
        /// </summary>
        /// <param name="url">Relative URL without parameters</param>
        /// <returns></returns>
        public static MultiDocumentQuery GetNodeBySeoUrlQuery(string url)
        {
            return DocumentHelper.GetDocuments()
                .Types(GetPageTypesWithSeoUrlClassNames())
                .Columns("SeoUrl", "DocumentID")
                .WhereEquals("SeoUrl", url)
                .TopN(1);
        }

        private static string[] GetPageTypesWithSeoUrlClassNames()
        {
            const string cacheKey = "deleteboilerplate|pagetypeswithseourlclassnames|all";

            var result = new string[0];
            var siteName = SiteContext.CurrentSiteName;

            using (var cs = new CachedSection<string[]>(ref result, CacheHelper.CacheMinutes(siteName), true, cacheKey))
            {
                if (cs.LoadData)
                {            
                    // All page types
                    var allPageTypes = DataClassInfoProvider.GetClasses()
                        .Where(dataClass => dataClass.ClassIsDocumentType)
                        .ToList();

                    var basePageType = allPageTypes
                        .FirstOrDefault(x => x.ClassName == BasePage.CLASS_NAME);

                    // All page types with SeoUrl column
                    result = allPageTypes
                        .Where(x => x.ClassInheritsFromClassID == basePageType?.ClassID)
                        .Select(x => x.ClassName)
                        .ToArray();

                    var cacheDependencies = new List<string>
                    {
                        "cms.classes|all"
                    };

                    cs.Data = result;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return result;
        }
    }
}