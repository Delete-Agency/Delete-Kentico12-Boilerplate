using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using CMS.SiteProvider;
using System.Collections.Generic;
using System.Linq;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface ISocialLinksRepository : IRepository<SocialIcon>
    {
        List<SocialIcon> GetAllSocialIcons(string siteName = null);
    }
    public class SocialLinksRepository : ISocialLinksRepository
    {
        private readonly string _projectCacheKey = "deleteboilerplate|socialicon";

        public List<SocialIcon> GetAllSocialIcons(string siteName = null)
        {
            var socialLinks = new List<SocialIcon>();

            if (siteName == null)
            {
                siteName = SiteContext.CurrentSiteName;
            }

            using (var cs = new CachedSection<List<SocialIcon>>(ref socialLinks, CacheHelper.CacheMinutes(siteName), true, _projectCacheKey))
            {
                if (cs.LoadData)
                {
                    socialLinks = SocialIconProvider.GetSocialIcons().OnSite(siteName).ToList();

                    var cacheDependencies = new List<string>
                    {
                        $"nodes|{siteName}|{SocialIcon.CLASS_NAME}|all"
                    };

                    cs.Data = socialLinks;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return socialLinks;
        }
    }
}
