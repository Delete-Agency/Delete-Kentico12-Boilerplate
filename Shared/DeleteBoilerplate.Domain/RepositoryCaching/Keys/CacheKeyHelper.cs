using CMS.SiteProvider;
using System.Collections;
using System.Globalization;
using System.Text;

namespace DeleteBoilerplate.Domain.RepositoryCaching.Keys
{
    public class CacheKeyParams
    {
        public string Type { get; set; }

        public string Method { get; set; }

        public object[] Arguments { get; set; }
    }

    public class CacheKeyHelper
    {
        public static string GetCacheItemKey(CacheKeyParams cacheKeyParams)
        {
            var builder = new StringBuilder(127)
                .Append("Custom")
                .Append("|").Append(SiteContext.CurrentSiteName)
                .Append("|").Append(cacheKeyParams.Type)
                .Append("|").Append(cacheKeyParams.Method)
                .Append("|").Append(CultureInfo.CurrentCulture.Name);

            foreach (var value in cacheKeyParams.Arguments ?? new object[] { })
            {
                var argumentCacheKey = GetArgumentCacheKey(value);

                if (argumentCacheKey == null)
                    return null;

                builder.AppendFormat(CultureInfo.InvariantCulture, "|{0}", argumentCacheKey);
            }

            return builder.ToString();
        }

        private static string GetArgumentCacheKey(object argument)
        {
            // null here is a representation of value, for example for default parameters param=null
            if (argument == null)
                return "null";

            var argumentType = argument.GetType();

            if (argumentType.IsPrimitive || argumentType.IsValueType)
                return argument.ToString();

            switch (argument)
            {
                case ICacheKey keyArgument:
                    return keyArgument.GetCacheKey();
                case IEnumerable enumerableArgument:
                    return GetEnumerableCacheKey(enumerableArgument);
                default:
                    return null;
            }
        }

        private static string GetEnumerableCacheKey(IEnumerable argument)
        {
            var cacheKey = string.Empty;

            foreach (var element in argument)
            {
                var elementKey = GetArgumentCacheKey(element);
                cacheKey += elementKey;
            }

            return cacheKey;
        }
    }
}
