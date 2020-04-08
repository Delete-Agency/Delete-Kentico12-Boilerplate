using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Common.Extensions
{
    public static class StringExtensions
    {
        public static string GetAbsoluteUrl(this string relativeUrl)
        {
            var domain = SiteContext.CurrentSite.DomainName;
            var absoluteUrl = URLHelper.GetAbsoluteUrl(relativeUrl, domain);

            return URLHelper.RemovePortFromURL(absoluteUrl);
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string OrDefault(this string value, string defaultValue)
        {
            return !IsEmpty(value) ? value : defaultValue;
        }

        public static string IfEmpty(this string value, string ifEmptyValue)
        {
            return string.IsNullOrWhiteSpace(value)
                ? ifEmptyValue
                : value;
        }
    }
}
