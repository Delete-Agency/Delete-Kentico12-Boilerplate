using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Common.Models.Media;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

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

        public static HtmlString RenderImageLazy<TModel>(this HtmlHelper<TModel> htmlHelper,
            Func<TModel, object> field, object parameters = null)
        {
            var imageField = field.Invoke(htmlHelper.ViewData.Model) as ImageViewModel;
            return htmlHelper.RenderImageLazy(imageField, parameters);
        }

        public static HtmlString RenderImageLazy(this HtmlHelper htmlHelper,
            ImageViewModel imageField, object parameters = null)
        {
            if (imageField == null || string.IsNullOrEmpty(imageField.Url))
                return new HtmlString(string.Empty);

            var attributes = parameters.GetHtmlAttributeCollection().ToDictionary();

            int[] sizes = null;
            string sizesStr = null;

            if (attributes.ContainsKey("data-srcset"))
                sizesStr = attributes["data-srcset"];
            else if (attributes.ContainsKey("sizes"))
                sizesStr = attributes["sizes"];

            if (!sizesStr.IsNullOrEmpty())
            {
                sizes = sizesStr.ParseIntArray().ToArray();
            }

            if (!attributes.ContainsKey("class"))
                attributes.Add("class", "lazyload");
            else
                attributes["class"] = attributes["class"].Append("lazyload", " ");
            attributes["data-srcset"] = htmlHelper.GetResizedSrcSet(imageField.Url, sizes);
            attributes["data-sizes"] = "auto";
            attributes["alt"] = attributes.ContainsKey("alt") ? attributes["alt"] : imageField.Title;

            return BuildImageTag(attributes);
        }

        private static HtmlString BuildImageTag(IDictionary<string, string> attributes)
        {
            var builder = new TagBuilder("img");
            foreach (var keyValuePair in attributes)
            {
                builder.Attributes.Add(keyValuePair.Key, keyValuePair.Value);
            }

            var tagResult = new HtmlString(builder.ToString(TagRenderMode.SelfClosing));

            return tagResult;
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

        /// <summary>
        /// Returns localization for given resource string key in a preferred UI culture of current user.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="key">Resource string key.</param>
        /// <remarks>Returned string is not HTML encoded.</remarks>
        public static IHtmlString GetUIString(this HtmlHelper htmlHelper, string key)
        {
            return htmlHelper.Raw(ResHelper.GetString(key, MembershipContext.AuthenticatedUser.PreferredUICultureCode));
        }

        /// <summary>
        /// Generates an IMG tag for an image file.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="path">The virtual path of the image.</param>
        /// <param name="title">Title.</param>
        /// <param name="cssClassName">CSS class.</param>
        /// <param name="constraint">Size constraint.</param>
        public static MvcHtmlString Image(this HtmlHelper htmlHelper, string path, string title = "", string cssClassName = "", SizeConstraint? constraint = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return MvcHtmlString.Empty;
            }

            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var image = new TagBuilder("img");
            image.MergeAttribute("src", urlHelper.Kentico().ImageUrl(path, constraint.GetValueOrDefault(SizeConstraint.Empty)));
            image.AddCssClass(cssClassName);
            image.MergeAttribute("alt", title);
            image.MergeAttribute("title", title);

            return MvcHtmlString.Create(image.ToString(TagRenderMode.SelfClosing));
        }
    }
}
