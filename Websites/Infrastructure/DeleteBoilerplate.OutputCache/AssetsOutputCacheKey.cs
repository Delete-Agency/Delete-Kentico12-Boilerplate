using System.Web;
using Kentico.Web.Mvc;
using HtmlHelperExtensions = DeleteBoilerplate.Infrastructure.Extensions.HtmlHelperExtensions;

namespace DeleteBoilerplate.OutputCache
{
    public class AssetsOutputCacheKey : IOutputCacheKey
    {
        public string Name => nameof(AssetsOutputCacheKey);

        public string GetVaryByCustomString(HttpContextBase context, string custom)
        {
            var assetsCookieValue = context.Request.Cookies[HtmlHelperExtensions.AssetsCookieName]?.Value ?? "empty";
            return $"{Name}={assetsCookieValue}";
        }
    }
}
