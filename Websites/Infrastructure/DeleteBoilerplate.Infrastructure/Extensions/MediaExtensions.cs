using System;
using System.Text.RegularExpressions;
using CMS.EventLog;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using DeleteBoilerplate.Infrastructure.Models.Media;
using Kentico.Components.Web.Mvc.FormComponents;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class MediaExtensions
    {
        public static string GetMediaFilesSelectorItemUrl(this MediaFilesSelectorItem mediaFilesSelectorItem)
        {
            var mediaFile = MediaFileInfoProvider.GetMediaFileInfo(mediaFilesSelectorItem.FileGuid, SiteContext.CurrentSiteName);
            return MediaLibraryHelper.GetDirectUrl(mediaFile);
        }

        public static ImageModel GetImageModel(this MediaFilesSelectorItem mediaFilesSelectorItem) =>
            GetImageModel(mediaFilesSelectorItem.FileGuid);

        private static ImageModel GetImageModel(Guid id)
        {
            try
            {
                var mediaFile = MediaFileInfoProvider.GetMediaFileInfo(id, SiteContext.CurrentSiteName);

                var url = MediaFileURLProvider.GetMediaFileUrl(mediaFile?.FileGUID ?? Guid.Empty, $"{mediaFile?.FileName}{mediaFile?.FileExtension}");

                return new ImageModel
                {
                    Id = id,
                    Title = mediaFile?.FileDescription.IfEmpty(mediaFile.FileName) ?? mediaFile?.FileDescription,
                    Url = url,
                    FileName = $"{mediaFile?.FileName}{mediaFile?.FileExtension}",
                    FileExtension = mediaFile?.FileExtension.Replace(".", string.Empty),
                    UploadDate = mediaFile?.FileCreatedWhen
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("MediaExtensions", "EXCEPTION", ex);
                return null;
            }
        }

        public static ImageModel GetImageModelByURl(string url)
        {
            var match = Regex.Match(url, @"[0-9a-fA-F]{8}[-]([0-9a-fA-F]{4}[-]){3}[0-9A-Fa-f]{12}");
            if (match.Success)
            {
                return GetImageModel(Guid.Parse(match.Value));
            }

            return null;
        }
    }
}
