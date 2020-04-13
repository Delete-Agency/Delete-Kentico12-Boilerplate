using System.Collections.Generic;
using System.Collections.Specialized;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var key in collection.AllKeys)
            {
                dictionary.Add(key, collection[key]);
            }

            return dictionary;
        }
    }
}