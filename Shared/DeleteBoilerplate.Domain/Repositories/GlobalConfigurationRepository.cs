using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IGlobalConfigurationRepository
    {
        GlobalConfiguration GetGlobalConfiguration();
    }

    public class GlobalConfigurationRepository : IGlobalConfigurationRepository
    {
        private readonly string _baseCacheKey = "deleteboilerplate|globalconfiguration";

        public GlobalConfiguration GetGlobalConfiguration()
        {
            var cacheKey = $"{_baseCacheKey}|singleton";

            var result = new GlobalConfiguration();

            using (var cs = new CachedSection<GlobalConfiguration>(ref result, CacheHelper.CacheMinutes(SiteContext.CurrentSiteName), true, cacheKey))
            {
                if (cs.LoadData)
                {
                    result = GlobalConfigurationProvider.GetGlobalConfigurations().OnSite(SiteContext.CurrentSiteName)
                        .FirstOrDefault();

                    var cacheDependencies = new List<string>
                    {
                        $"nodes|{SiteContext.CurrentSiteName}|{GlobalConfiguration.CLASS_NAME}|all"
                    };

                    cs.Data = result;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return result;
        }
    }
}
