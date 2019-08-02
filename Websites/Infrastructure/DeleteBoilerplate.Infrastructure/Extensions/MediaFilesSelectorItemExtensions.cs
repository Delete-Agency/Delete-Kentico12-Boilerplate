using System;
using CMS.EventLog;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using DeleteBoilerplate.Infrastructure.Models.Media;
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

        public static ImageModel GetImageModel(this MediaFilesSelectorItem mediaFilesSelectorItem)
        {
            try
            {
                var mediaFile = MediaFileInfoProvider.GetMediaFileInfo(mediaFilesSelectorItem.FileGuid, SiteContext.CurrentSiteName);

                var url = MediaFileURLProvider.GetMediaFileUrl(mediaFile?.FileGUID ?? Guid.Empty, $"{mediaFile?.FileName}{mediaFile?.FileExtension}");

                return new ImageModel
                {
                    Id = mediaFilesSelectorItem.FileGuid,
                    Title = mediaFile?.FileDescription.IfEmpty(mediaFile.FileName) ?? mediaFile?.FileDescription,
                    Url = url,
                    FileName = $"{mediaFile?.FileName}{mediaFile?.FileExtension}",
                    FileExtension = mediaFile?.FileExtension.Replace(".", string.Empty),
                    UploadDate = mediaFile?.FileCreatedWhen
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("MediaFilesSelectorItemExtensions", "EXCEPTION", ex);
                return null;
            }
        }
    }
}
