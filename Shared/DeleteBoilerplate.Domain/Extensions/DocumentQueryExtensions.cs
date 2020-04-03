using CMS.DocumentEngine;
using CMS.Localization;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Domain.Extensions
{
    public static class DocumentQueryExtensions
    {
        public static DocumentQuery<T> AddVersionsParameters<T>(this DocumentQuery<T> query, bool? isPreview = null) where T: TreeNode, new()
        {
            if (isPreview == null)
                isPreview = Settings.PreviewEnabled;

            return query
                .LatestVersion(isPreview.Value)
                .Published(!isPreview.Value)
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(LocalizationContext.CurrentCulture.CultureCode);
        }

        public static MultiDocumentQuery AddVersionsParameters(this MultiDocumentQuery query, bool? isPreview = null)
        {
            if (isPreview == null)
                isPreview = Settings.PreviewEnabled;

            return query
                .LatestVersion(isPreview.Value)
                .Published(!isPreview.Value)
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(LocalizationContext.CurrentCulture.CultureCode);
        }
    }
}