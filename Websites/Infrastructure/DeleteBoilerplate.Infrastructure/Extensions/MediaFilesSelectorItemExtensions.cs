using CMS.MediaLibrary;
using CMS.SiteProvider;
using Kentico.Components.Web.Mvc.FormComponents;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class MediaFilesSelectorItemExtensions
    {
        public static string GetMediaFilesSelectorItemUrl(this MediaFilesSelectorItem mediaFilesSelectorItem)
        {
            var mediaFile = MediaFileInfoProvider.GetMediaFileInfo(mediaFilesSelectorItem.FileGuid, SiteContext.CurrentSiteName);
            return MediaLibraryHelper.GetDirectUrl(mediaFile);
        }
    }
}
