using System;
using System.Collections.Generic;
using System.Linq;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.SiteProvider;

namespace DeleteBoilerplate.XmlSitemap.Helpers
{
    public static class CustomClassHelper
    {
        // (skip, take) => lambda
        private static readonly Dictionary<string, Func<int, int, IQueryable<BaseInfo>>> QueryDictionary
            = new Dictionary<string, Func<int, int, IQueryable<BaseInfo>>>
            {
            };

        private static readonly Dictionary<Type, IList<Func<BaseInfo, string>>> UrlDictionary
            = new Dictionary<Type, IList<Func<BaseInfo, string>>>
            {
            };

        public static IQueryable<BaseInfo> GetQuery(string className, int skip, int take)
        {
            if (!QueryDictionary.ContainsKey(className))
            {
                EventLogProvider
                    .LogWarning(nameof(CustomClassHelper),
                        "NOT_IMPL_SITEMAP",
                        null,
                        SiteContext.CurrentSiteID,
                        $"Attempt to get query for sitemap generate task for not implemented custom class: {className}");

                return null;
            }

            var func = QueryDictionary[className];

            return func.Invoke(skip, take);
        }

        public static IList<string> GetUrls(BaseInfo instance)
        {
            var className = instance.TypeInfo.ObjectClassName;

            var result = new List<string>();

            if (!UrlDictionary.ContainsKey(instance.GetType()))
            {
                EventLogProvider
                    .LogWarning(nameof(CustomClassHelper),
                        "NOT_IMPL_SITEMAP",
                        null,
                        SiteContext.CurrentSiteID,
                        $"Attempt to to get url for sitemap generate task for not implemented custom class: {className}");

                return result;
            }

            var funcList = UrlDictionary[instance.GetType()];

            result.AddRange(funcList.Select(func => func.Invoke(instance)));

            return result;
        }
    }
}
