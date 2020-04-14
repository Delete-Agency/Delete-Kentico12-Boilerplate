using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CMS.EventLog;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Models.Media;

namespace DeleteBoilerplate.Common.Extensions
{
    public static class MediaExtensions
    {
        public static ImageModel GetImageModel(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            try
            {
                ImageModel result = null;
                var cacheKey = $"deleteboilerplate|mediaitem|{id}";

                using (var cs = new CachedSection<ImageModel>(ref result, CacheHelper.CacheMinutes(SiteContext.CurrentSiteName), true, cacheKey))
                {
                    if (cs.LoadData)
                    {
                        var mediaFile = MediaFileInfoProvider.GetMediaFileInfo(id, SiteContext.CurrentSiteName);

                        var url = MediaFileURLProvider.GetMediaFileUrl(mediaFile?.FileGUID ?? Guid.Empty, $"{mediaFile?.FileName}{mediaFile?.FileExtension}");

                        result = new ImageModel
                        {
                            Id = id,
                            Title = mediaFile?.FileTitle.IfEmpty(mediaFile?.FileName),
                            Url = URLHelper.ResolveUrl(url),
                            FileName = $"{mediaFile?.FileName}{mediaFile?.FileExtension}",
                            FileExtension = mediaFile?.FileExtension.Replace(".", string.Empty),
                            UploadDate = mediaFile?.FileCreatedWhen,
                            Width = mediaFile?.FileImageWidth,
                            Height = mediaFile?.FileImageHeight,
                            Order = mediaFile?.GetValue<long?>("Order", null)
                        };

                        var cacheDependencies = new List<string>
                        {
                            $"mediafile|{id}"
                        };

                        cs.Data = result;
                        cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                    }
                }

                return result;
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
