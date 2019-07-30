using System;
using System.Collections.Generic;
using System.Linq;
using DeleteBoilerplate.Infrastructure.Attributes;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static string GetStringValue<TEnum>(this TEnum value) where TEnum : struct
        {
            return value.GetStringValue(null);
        }

        public static string GetStringValue<TEnum>(this TEnum value, string key) where TEnum : struct
        {
            var attributes = value.GetCustomAttributes<TEnum, StringValueAttribute>();
            var targetAttribute = !string.IsNullOrEmpty(key)
                ? attributes.FirstOrDefault(a => a.Key != null && a.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                : attributes.FirstOrDefault();

            return targetAttribute != null
                ? targetAttribute.Value
                : string.Empty;
        }

        public static IEnumerable<TAttr> GetCustomAttributes<TEnum, TAttr>(this TEnum value)
            where TEnum : struct
            where TAttr : Attribute
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = Attribute.GetCustomAttributes(field, typeof(TAttr)).Cast<TAttr>();
            return attributes;
        }
    }
}
