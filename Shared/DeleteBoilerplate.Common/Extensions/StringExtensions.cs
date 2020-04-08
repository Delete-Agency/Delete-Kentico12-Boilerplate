using CMS.Helpers;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Models;
using System;
using System.Collections.Generic;

namespace DeleteBoilerplate.Common.Extensions
{
    public static class StringExtensions
    {
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

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string[] SplitByNewline(this string value, StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
        {
            var newLineSymbols = new[] { "\r\n", "\r", "\n" };

            if (value.IsNullOrWhiteSpace())
                return new string[0];

            return value.Split(newLineSymbols, splitOptions);
        }

        public static IEnumerable<int> ParseIntArray(this string value, char separator = ',')
        {
            return value.ParseArray(separator, s =>
            {
                var itemResult = new ParseResult<int>
                {
                    Successful = int.TryParse(s, out var parsedItem),
                    Value = parsedItem
                };

                return itemResult;
            });
        }

        public static IEnumerable<Guid> ParseGuidArray(this string value, char separator = '|')
        {
            return value.ParseArray(separator, s =>
            {
                var itemResult = new ParseResult<Guid>
                {
                    Successful = Guid.TryParse(s, out var parsedItem),
                    Value = parsedItem
                };

                return itemResult;
            });
        }

        public static IEnumerable<TElement> ParseArray<TElement>(this string value, char separator, Func<string, ParseResult<TElement>> itemParser)
        {
            if (itemParser == null)
            {
                throw new ArgumentNullException(nameof(itemParser));
            }

            var result = new List<TElement>();

            if (string.IsNullOrEmpty(value)) 
                return result;

            foreach (var item in value.Split(separator))
            {
                try
                {
                    var itemParseResult = itemParser(item);
                    if (itemParseResult.Successful)
                    {
                        result.Add(itemParseResult.Value);
                    }
                }
                catch (OverflowException)
                {
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }

        public static string FirstNotEmpty(params string[] strings)
        {
            foreach (var str in strings)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    return str;
                }
            }

            return string.Empty;
        }

        public static string TruncateText(this string text, int length, bool ellipsisEnd = false)
        {
            if (text.IsNullOrWhiteSpace() || text.Length < length)
                return text;

            text = text.Substring(0, ellipsisEnd ? length - 3 : length);

            text = text.TrimEnd('\n', '\r', ' ', '-', '—');

            if (ellipsisEnd)
            {
                text = text.TrimEnd('.', '?', '!');
                text += "...";
            }

            return text;
        }

        public static string GetAbsoluteUrl(this string relativeUrl)
        {
            var domain = SiteContext.CurrentSite.DomainName;
            var absoluteUrl = URLHelper.GetAbsoluteUrl(relativeUrl, domain);

            var isAbsoluteUri = new Uri(absoluteUrl, UriKind.RelativeOrAbsolute).IsAbsoluteUri;
            if (!isAbsoluteUri)
            {
                absoluteUrl = GetAbsoluteUrlUsingSitePresentationUrl(relativeUrl);
            }

            return URLHelper.RemovePortFromURL(absoluteUrl);
        }

        private static string GetAbsoluteUrlUsingSitePresentationUrl(this string relativeUrl)
        {
            var sitePresentationUrl = SiteContext.CurrentSite.SitePresentationURL;

            sitePresentationUrl = sitePresentationUrl.TrimEnd('/');

            if (relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.TrimStart('~');

            if (!relativeUrl.StartsWith("/"))
                relativeUrl = $"/{relativeUrl}";

            return $"{sitePresentationUrl}{relativeUrl}";
        }
    }
}
