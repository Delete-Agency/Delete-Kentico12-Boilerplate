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
        // Key = Custom Class CLASSNAME
        // Value - Func returning query from Provider where (skip, take) => lambda
        private static readonly Dictionary<string, Func<int, int, IQueryable<BaseInfo>>> QueryDictionary
            = new Dictionary<string, Func<int, int, IQueryable<BaseInfo>>>
            {
                {
                    CompanyMemberInfo.TYPEINFO.ObjectClassName,
                    (skip, take) => CompanyMemberInfoProvider
                        .GetCompanyMembers()
                        .OrderBy(x => x.CompanyMemberID)
                        .Skip(skip)
                        .Take(take)
                }
            };

        // Key - Custom Class C# Type
        // Value - Collection of string URLs (starts with '/', without trailing '/')
        private static readonly Dictionary<Type, IList<Func<BaseInfo, string>>> UrlDictionary
            = new Dictionary<Type, IList<Func<BaseInfo, string>>>
            {
                {
                    typeof(CompanyMemberInfo),
                    new List<Func<BaseInfo, string>>
                    {
                        info =>
                        {
                            var companyMemberInfo = (CompanyMemberInfo)info;

                            return $"/team/{companyMemberInfo.Team}/{companyMemberInfo.PersonalIdentifier}";
                        }
                    }
                }
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
