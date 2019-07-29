using System;
using CMS.Helpers;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string GetAbsoluteAppUrl(this string relativeUrl)
        {
            var appUrl = URLHelper.GetApplicationUrl();

            relativeUrl = relativeUrl.EnsureStringStartsWith("/");

            return $"{appUrl}{relativeUrl}";
        }

        public static string EnsureStringStartsWith(this string source, string pattern)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;

            return source.StartsWith(pattern, StringComparison.OrdinalIgnoreCase) ? source : $"{pattern}{source}";
        }
    }
}