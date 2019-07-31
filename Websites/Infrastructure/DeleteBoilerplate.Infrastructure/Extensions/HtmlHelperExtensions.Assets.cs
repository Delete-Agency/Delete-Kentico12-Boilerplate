using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DeleteBoilerplate.Infrastructure.Models;
using System.Web.Mvc;
using CMS.EventLog;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public enum ContentType
    {
        Undefined = 0,
        CSS = 1,
        JS = 2,
        Svg = 4,
        ModernJS = 8,
    }

    public enum AssetRendering
    {
        FrontendOnly = 0,
        AdminOnly = 1,
        FrontendAndAdmin = 2
    }

    public static partial class HtmlHelperExtensions
    {
        public const string AssetsCookieName = "assets";
        private static readonly Regex IsLocalPattern = new Regex(@"^((http(s)?):)?\/\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static string CurrentContextKeys(ContentType type) => $"currentContext_{type}";

        private static readonly Dictionary<ContentType, string> ContentPatternsInline = new Dictionary<ContentType, string>
        {
            { ContentType.CSS, "<style>{0}</style>\n" },
            { ContentType.JS, "<script>{0}</script>\n" },
            { ContentType.Svg, "{0}\n" },
        };

        public static IList<StaticAsset> GetRegisteredComponents(this HtmlHelper htmlHelper, ContentType contentType)
        {
            return htmlHelper.GetRegisteredAssets(contentType).OrderBy(x => x.Order).ThenByDescending(x => x.IsLocal).ToList();
        }

        private static IEnumerable<StaticAsset> GetRegisteredAssets(this HtmlHelper htmlHelper, ContentType contentType)
        {
            return GetAssetDict(contentType)?.Select(x => x.Value).ToList() ?? Enumerable.Empty<StaticAsset>();
        }

        private static Dictionary<string, StaticAsset> GetAssetDict(string key)
        {
            lock (HttpContext.Current)
            {
                if (HttpContext.Current.Items[key] as Dictionary<string, StaticAsset> == null)
                {
                    HttpContext.Current.Items[key] = new Dictionary<string, StaticAsset>();
                }
            }
            return HttpContext.Current.Items[key] as Dictionary<string, StaticAsset>;
        }

        private static Dictionary<string, StaticAsset> GetAssetDict(ContentType contentType)
        {
            return GetAssetDict(CurrentContextKeys(contentType));
        }

        public static bool IsLocal(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return !IsLocalPattern.IsMatch(url);
        }

        public static bool IsSameAssetsCacheCookie()
        {
            var key = HttpContext.Current.Request.Cookies[AssetsCookieName];
            return key?.Value == ManifestHash;
        }


        public static MvcHtmlString SetAssetsCacheCookie(this HtmlHelper htmlHelper)
        {
            var cookie = new HttpCookie(AssetsCookieName, ManifestHash) { Expires = DateTime.Now.AddDays(365), Secure = false };
            HttpContext.Current.Response.Cookies.Add(cookie);
            return new MvcHtmlString(string.Empty);
        }

        private static string ConvertEntryToModern(string x)
        {
            return !IsLocal(x) ? x : (x ?? string.Empty).Replace(".js", ".mjs");
        }

        public static void RegisterComponent(ContentType contentType, string fileName, int order = -1)
        {
            if (fileName.IsEmpty()) return;

            var dictionary = GetAssetDict(contentType);
            lock (dictionary)
            {
                if (dictionary.ContainsKey(fileName)) return;
                var staticAsset = CreateStaticAsset(fileName, order);
                if (string.IsNullOrWhiteSpace(staticAsset.Path))
                {
                    EventLogProvider.LogException("Frontend", "ASSETNOTFOUND", new Exception($"Entry point \"{fileName}\" not found!"));
                    return;
                }
                dictionary.Add(fileName, staticAsset);
            }

            if (contentType == ContentType.JS && IsLocal(fileName))
            {
                RegisterComponent(ContentType.ModernJS, ConvertEntryToModern(fileName), order);
            }
        }

        private static StaticAsset CreateStaticAsset(string x, int order = -1)
        {
            var asset = new StaticAsset { Name = x, Path = GetWebPath(x), };
            if (order >= 0) asset.Order = order;
            return asset;
        }

        public static MvcHtmlString InlineCSS(this HtmlHelper htmlHelper, params string[] assets) =>
            Inline(htmlHelper, ContentType.CSS, assets);
        public static MvcHtmlString InlineSVG(this HtmlHelper htmlHelper, params string[] assets) =>
            Inline(htmlHelper, ContentType.Svg, assets);
        private static MvcHtmlString Inline(this HtmlHelper htmlHelper,ContentType type, IEnumerable<string> assets)
        {
            var builder = new StringBuilder();
            foreach (var asset in assets)
            {
                builder.Append(GetContent(asset));
            }

            var str = builder.ToString();
            return new MvcHtmlString(str.IsEmpty() ? str : string.Format(ContentPatternsInline.GetValueOrDefault(type), str));
        }


        public static MvcHtmlString RegisterScript(this HtmlHelper htmlHelper, string fileName,
            AssetRendering renderWhen = AssetRendering.FrontendOnly, int order = 100) =>
            RegisterAsset(htmlHelper, ContentType.JS, fileName, renderWhen, order);

        public static MvcHtmlString RegisterStyle(this HtmlHelper htmlHelper, string fileName,
            AssetRendering renderWhen = AssetRendering.FrontendOnly, int order = 100) =>
            RegisterAsset(htmlHelper, ContentType.CSS, fileName, renderWhen, order);

        public static MvcHtmlString RegisterSvg(this HtmlHelper htmlHelper, string fileName,
            AssetRendering renderWhen = AssetRendering.FrontendOnly, int order = 100) =>
            RegisterAsset(htmlHelper, ContentType.Svg, fileName, renderWhen, order);

        private static MvcHtmlString RegisterAsset(this HtmlHelper htmlHelper, ContentType assetType, string fileName, AssetRendering renderWhen, int order)
        {
            if (!htmlHelper.ViewContext.HttpContext.Kentico().Preview().Enabled && renderWhen == AssetRendering.AdminOnly ||
                htmlHelper.ViewContext.HttpContext.Kentico().Preview().Enabled && renderWhen == AssetRendering.FrontendOnly)
            {
                return MvcHtmlString.Empty;
            }

            RegisterComponent(assetType, fileName, order);

            return MvcHtmlString.Create(string.Empty);
        }


    }
}