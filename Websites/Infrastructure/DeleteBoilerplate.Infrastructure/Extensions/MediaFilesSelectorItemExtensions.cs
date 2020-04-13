using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Common.Models.Media;
using Kentico.Components.Web.Mvc.FormComponents;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class MediaFilesSelectorItemExtensions
    {
        public static string GetMediaFilesSelectorItemUrl(this MediaFilesSelectorItem mediaFilesSelectorItem)
        {
            var mediaFile = MediaFileInfoProvider.GetMediaFileInfo(mediaFilesSelectorItem.FileGuid, SiteContext.CurrentSiteName);

            if (mediaFile != null)
                return URLHelper.ResolveUrl(MediaLibraryHelper.GetDirectUrl(mediaFile));

            return null;
        }

        public static ImageModel GetImageModel(this MediaFilesSelectorItem mediaFilesSelectorItem) => MediaExtensions.GetImageModel(mediaFilesSelectorItem.FileGuid);
    }
}
