using CMS.DocumentEngine;
using CMS.SiteProvider;

namespace DeleteBoilerplate.DynamicRouting.Extensions
{
    public static class DocumentQueryExtensions
    {
        public static DocumentQuery<T> AddVersionsParameters<T>(this DocumentQuery<T> query, bool isPreview) where T: TreeNode, new()
        {
            return query
                .LatestVersion(isPreview)
                .Published(!isPreview)
                .OnSite(SiteContext.CurrentSiteName);
        }

        public static MultiDocumentQuery AddVersionsParameters(this MultiDocumentQuery query, bool isPreview)
        {
            return query
                .LatestVersion(isPreview)
                .Published(!isPreview)
                .OnSite(SiteContext.CurrentSiteName);
        }
    }
}