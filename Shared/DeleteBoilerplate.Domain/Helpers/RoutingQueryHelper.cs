using System.Collections.Generic;
using System.Linq;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Domain.Helpers
{
    public class RoutingQueryHelper
    {
        /// <summary>
        /// Gets the query returning TreeNode for the corresponding url
        /// </summary>
        /// <param name="url">Relative URL without parameters</param>
        /// <param name="columns">Columns to include in the query</param>
        /// <returns></returns>
        public static MultiDocumentQuery GetNodeBySeoUrlQuery(string url, string[] columns = null)
        {
            return DocumentHelper.GetDocuments()
                .Types(GetPageTypesWithSeoUrlClassNames())
                .Columns(columns ?? new[] {Constants.DynamicRouting.SeoUrlFieldName, "DocumentID"})
                .WhereEquals(Constants.DynamicRouting.SeoUrlFieldName, url);
        }

        public static MultiDocumentQuery GetAllNodesWithSeoUrlQuery(string siteName = null)
        {
            return DocumentHelper.GetDocuments()
                .Types(GetPageTypesWithSeoUrlClassNames(siteName))
                .Columns(new[] { Constants.DynamicRouting.SeoUrlFieldName, "DocumentID" });
        }

        public static string[] GetPageTypesWithSeoUrlClassNames(string siteName = null)
        {
            const string cacheKey = "deleteboilerplate|pagetypeswithseourlclassnames|all";

            var result = new string[0];
            if (siteName == null)
            {
                siteName = SiteContext.CurrentSiteName;
            }

            using (var cs = new CachedSection<string[]>(ref result, CacheHelper.CacheMinutes(siteName), true, cacheKey))
            {
                if (cs.LoadData)
                {
                    // All page types with SeoUrl column
                    result = DataClassInfoProvider.GetClasses()
                        .Where(dataClass =>
                            dataClass.ClassIsDocumentType &&
                            dataClass.ClassSearchSettingsInfos.Any(x => x.Name.Equals(Constants.DynamicRouting.SeoUrlFieldName)))
                        .Select(x => x.ClassName)
                        .ToArray();

                    var cacheDependencies = new List<string>
                    {
                        "cms.class|all",
                        "cms.documenttype|all"
                    };

                    cs.Data = result;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return result;
        }
    }
}