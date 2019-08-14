using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CMS.DataEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.SiteProvider;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.DynamicRouting.Contexts;
using DeleteBoilerplate.GenericComponents.Models.Footer;
using DeleteBoilerplate.GenericComponents.Models.Header;
using LightInject;

namespace DeleteBoilerplate.GenericComponents.Controllers.Global
{
    public class GlobalController : Controller
    {
        [Inject]
        public ISocialLinksRepository SocialLinksRepository { get; set; }

        [Inject]
        public INavigationRepository NavigationLinksRepository { get; set; }

        [Inject]
        public IRequestContext RequestContext { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        // GET: Global
        public ActionResult Header()
        {
            var headerNavigatePath = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.HeaderNavigationPath");
            var navigationLinks =
                Mapper.Map<List<NavigationLinkViewModel>>(NavigationLinksRepository.GetNavigationLinksByPath(headerNavigatePath)
                    .OrderBy(x => x.NodeOrder));

            var activeLinks = navigationLinks.SelectMany(x => x.ChildLinks)
                .Union(navigationLinks)
                .Where(x => !string.IsNullOrWhiteSpace(x.AssociatedPagePath) &&
                            RequestContext.ContextItem.NodeAliasPath.StartsWith(x.AssociatedPagePath, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var activeLink = activeLinks.FirstOrDefault(x => x.AssociatedPagePath.Equals(activeLinks.Aggregate("",
                (max, cur) => max.Length > cur.AssociatedPagePath.Length ? max : cur.AssociatedPagePath)));
            if (activeLink != null) activeLink.IsActive = true;
            var socialLinks = SocialLinksRepository.GetAllSocialIcons().OrderBy(x => x.NodeOrder);

            var model = new HeaderViewModel
            {
                NavLinks = navigationLinks,
                SocLinks = Mapper.Map<List<SocialLinkViewModel>>(socialLinks),
                ActiveLink = activeLink
            };

            return PartialView("Header", model);
        }

        public ActionResult Footer()
        {
            var footerNavigationPath = SettingsKeyInfoProvider.GetValue($"{SiteContext.CurrentSiteName}.FooterNavigationPath");
            var socialLinks = SocialLinksRepository.GetAllSocialIcons().OrderBy(x => x.NodeOrder);
            var navigationLinks = NavigationLinksRepository.GetNavigationLinksByPath(footerNavigationPath).OrderBy(x => x.NodeOrder);

            var model = new FooterViewModel
            {
                SocialLinks = Mapper.Map<List<SocialLinkViewModel>>(socialLinks),
                NavigationLinks = Mapper.Map<List<NavigationLinkViewModel>>(navigationLinks)
            };

            return PartialView("Footer", model);
        }
    }
}