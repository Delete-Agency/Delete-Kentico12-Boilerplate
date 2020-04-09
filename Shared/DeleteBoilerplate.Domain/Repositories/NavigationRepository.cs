using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.SiteProvider;
using DeleteBoilerplate.Domain.Extensions;
using DeleteBoilerplate.Domain.RepositoryCaching.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface INavigationRepository : IRepository<NavigationLink>
    {
        IList<NavigationLink> GetAllNavigationLinks(string siteName = null);

        IList<NavigationLink> GetNavigationLinksByPath(string path);
    }

    public class NavigationRepository : INavigationRepository
    {
        public NavigationLink GetById(int id)
        {
            return NavigationLinkProvider
                .GetNavigationLinks()
                .WithID(id)
                .AddVersionsParameters()
                .TopN(1)
                .FirstOrDefault();
        }

        [RepositoryCaching]
        public IList<NavigationLink> GetAllNavigationLinks(string siteName = null)
        {
            if (siteName == null)
            {
                siteName = SiteContext.CurrentSiteName;
            }

            var navigationLinks = NavigationLinkProvider
                .GetNavigationLinks()
                .OnSite(siteName)
                .ToList();

            return navigationLinks;
        }

        [RepositoryCaching]
        public IList<NavigationLink> GetNavigationLinksByPath(string path)
        {
            var result = NavigationLinkProvider.GetNavigationLinks()
                .AddVersionsParameters()
                .Path(path)
                .ToList();

            var associatedPagePaths = DocumentHelper.GetDocuments()
                .Columns("NodeGUID,NodeAliasPath")
                .WhereIn("NodeGUID", result.Select(x => x.AssociatedPage).ToList())
                .ToList();

            foreach (var navigationLink in result)
            {
                navigationLink.AssociatedPagePath = associatedPagePaths
                    .FirstOrDefault(x => x.NodeGUID == navigationLink.AssociatedPage)?.NodeAliasPath;

                navigationLink.ChildLinks = result
                    .Where(x => x.NodeParentID == navigationLink.NodeID)
                    .OrderBy(x => x.NodeOrder)
                    .ToList();

                foreach (var childLink in navigationLink.ChildLinks)
                {
                    childLink.ParentLink = navigationLink;
                }
            }

            var navigationLinks = result
                .Where(x => x.ParentLink == default(NavigationLink))
                .OrderBy(x => x.NodeOrder)
                .ToList();

            return navigationLinks;
        }
    }
}
