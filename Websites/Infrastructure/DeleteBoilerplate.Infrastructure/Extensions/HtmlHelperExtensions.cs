using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static partial class HtmlHelperExtensions
    {
        public static bool NoCache(this HtmlHelper helper)
        {
#if _NOCACHE
			return true;
#else
            return false;
#endif
        }

        public static string GetResizedSrcSet(this HtmlHelper htmlHelper, string mediaUrl, IEnumerable<int> widths = null)
        {
            if (string.IsNullOrWhiteSpace(mediaUrl))
            {
                EventLogProvider.LogWarning("HtmlHelperExtensions", "RENDERIMAGE",
                    new ArgumentNullException(nameof(mediaUrl), "Empty media URL"), SiteContext.CurrentSiteID,
                    String.Empty);
                return mediaUrl;
            }

            if (widths == null)
            {
                widths = new[] { 320, 360, 640, 720, 960, 1280, 1440 };
            }

            var result = new StringBuilder();

            try
            {
                var absoluteMediaUrl = URLHelper.GetAbsoluteUrl(mediaUrl);
                var uri = new UriBuilder(absoluteMediaUrl);

                var query = HttpUtility.ParseQueryString(uri.Query);
                query.Remove("height");

                foreach (var width in widths)
                {
                    query["width"] = width.ToString();

                    uri.Query = query.ToString();
                    result.AppendFormat("{0} {1}w,", uri.Uri.PathAndQuery, width);
                }

                result.Length--;
                return result.ToString();
            }
            catch (Exception exception)
            {
                EventLogProvider.LogException("HtmlHelperExtensions", "RENDERIMAGE", exception);
                return mediaUrl;
            }
        }

    }
}
