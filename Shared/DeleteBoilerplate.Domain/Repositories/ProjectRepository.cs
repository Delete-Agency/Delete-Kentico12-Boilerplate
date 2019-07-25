using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IProjectRepository
    {
        List<Project> GetAllProjects(string siteName = null);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly string _projectCacheKey = "deleteboilerplate|project";

        public List<Project> GetAllProjects(string siteName = null)
        {
            var result = new List<Project>();
            if (siteName == null)
            {
                siteName = SiteContext.CurrentSiteName;
            }
            using (var cs = new CachedSection<List<Project>>(ref result, CacheHelper.CacheMinutes(siteName), true, _projectCacheKey))
            {
                if (cs.LoadData)
                {
                    result = ProjectProvider.GetProjects().OnSite(siteName).ToList();

                    var cacheDependencies = new List<string>
                    {
                        $"nodes|{siteName}|{Project.CLASS_NAME}|all"
                    };

                    cs.Data = result;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return result;
        }
    }
}
