using System;
using System.Collections.Generic;
using CMS.Helpers;
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
        Svg = 4
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

        private static string CssPatternRegular = "<link rel=\"stylesheet\" href=\"{0}\" />";
        private static string JsPatternRegular = "<script defer src=\"{0}\"></script>";


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

        public static string RegisterComponent(ContentType contentType, string fileName, int order = -1)
        {
            if (fileName.IsEmpty()) return string.Empty;

            var assetLink = string.Empty;
            var dictionary = GetAssetDict(contentType);
            lock (dictionary)
            {
                if (dictionary.ContainsKey(fileName)) return string.Empty;
                var staticAsset = CreateStaticAsset(fileName, order);
                if (string.IsNullOrWhiteSpace(staticAsset.Path))
                {
                    EventLogProvider.LogException("Frontend", "ASSETNOTFOUND", new Exception($"Entry point \"{fileName}\" not found!"));
                    return string.Empty;
                }
                dictionary.Add(fileName, staticAsset);
                if (contentType == ContentType.CSS)
                {
                    assetLink = IsSameAssetsCacheCookie() ? string.Format(CssPatternRegular,staticAsset.Path) : InlineCSS(fileName);
                }

                if (contentType == ContentType.Svg)
                {
                    assetLink = InlineSVG(fileName);
                }
            }

            return assetLink;
        }

        private static StaticAsset CreateStaticAsset(string x, int order = -1)
        {
            var asset = new StaticAsset { Name = x, Path = GetWebPath(x), };
            if (order >= 0) asset.Order = order;
            return asset;
        }

        public static string InlineCSS(params string[] assets) =>
            Inline(ContentType.CSS, assets);
        public static string InlineSVG(params string[] assets) =>
            Inline(ContentType.Svg, assets);
        private static string Inline(ContentType type, IEnumerable<string> assets)
        {
            var builder = new StringBuilder();
            foreach (var asset in assets)
            {
                builder.Append(GetContent(asset));
            }

            var str = builder.ToString();
            return str.IsEmpty() ? str : string.Format(ContentPatternsInline.GetValueOrDefault(type), str);
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

            return MvcHtmlString.Create(RegisterComponent(assetType, fileName, order));
        }

        public static MvcHtmlString RenderRegisteredScripts(this HtmlHelper htmlHelper)
        {
            var combinedScripts = GetAssetDict(ContentType.JS).Select(x=>string.Format(JsPatternRegular, x.Value.Path)).Join("\n") ?? String.Empty;

            return MvcHtmlString.Create(combinedScripts);
        }

    }
}