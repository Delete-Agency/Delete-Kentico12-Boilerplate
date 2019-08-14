using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using CMS.SiteProvider;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface INavigationRepository
    {
        List<NavigationLink> GetAllNavigationLinks(string siteName = null);
        List<NavigationLink> GetNavigationLinksByPath(string path);
    }
    public class NavigationRepository : INavigationRepository
    {
        private readonly string _projectCacheKey = "deleteboilerplate|navigation";

        public List<NavigationLink> GetAllNavigationLinks(string siteName = null)
        {
            var navigationLinks = new List<NavigationLink>();

            if (siteName == null)
            {
                siteName = SiteContext.CurrentSiteName;
            }

            using (var cs = new CachedSection<List<NavigationLink>>(ref navigationLinks, CacheHelper.CacheMinutes(siteName), true, _projectCacheKey))
            {
                if (cs.LoadData)
                {
                    navigationLinks = NavigationLinkProvider.GetNavigationLinks().OnSite(siteName).ToList();

                    var cacheDependencies = new List<string>
                    {
                        $"nodes|{siteName}|{NavigationLink.CLASS_NAME}|all"
                    };

                    cs.Data = navigationLinks;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return navigationLinks;
        }

        public List<NavigationLink> GetNavigationLinksByPath(string path)
        {
            var cacheKey = $"{_projectCacheKey}|{path}";

            var navigationLinks = new List<NavigationLink>();

            using (var cs = new CachedSection<List<NavigationLink>>(ref navigationLinks, CacheHelper.CacheMinutes(SiteContext.CurrentSiteName), true, cacheKey))
            {
                if (cs.LoadData)
                {
                    var result = NavigationLinkProvider.GetNavigationLinks()
                        .Path(path)
                        .OnSite(SiteContext.CurrentSiteName)
                        .ToList();

                    var associatedPagePaths = DocumentHelper.GetDocuments()
                        .Columns("NodeGUID,NodeAliasPath")
                        .WhereIn("NodeGUID", result.Select(x => x.AssociatedPage).ToList())
                        .ToList();

                    foreach (var navigationLink in result)
                    {
                        navigationLink.AssociatedPagePath =
                            associatedPagePaths.FirstOrDefault(x => x.NodeGUID == navigationLink.AssociatedPage)
                                ?.NodeAliasPath;

                        navigationLink.ChildLinks = result.Where(x => x.NodeParentID == navigationLink.NodeID)
                            .OrderBy(x => x.NodeOrder)
                            .ToList();
                        foreach (var childLink in navigationLink.ChildLinks)
                        {
                            childLink.ParentLink = navigationLink;
                        }
                    }

                    navigationLinks = result.Where(x => x.ParentLink == default(NavigationLink)).ToList();

                    var cacheDependencies = new List<string>
                    {
                        $"nodes|{SiteContext.CurrentSiteName}|{NavigationLink.CLASS_NAME}|all"
                    };

                    cs.Data = navigationLinks;
                    cs.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);
                }
            }

            return navigationLinks;
        }
    }
}
