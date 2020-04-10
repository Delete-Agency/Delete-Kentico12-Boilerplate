using AutoMapper;
using CMS.Helpers;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.WebApp.Models.Global.Footer;
using DeleteBoilerplate.WebApp.Models.Global.Header;
using LightInject;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeleteBoilerplate.WebApp.Controllers.Global
{
    public class GlobalController : Controller
    {
        [Inject]
        public INavigationRepository NavigationLinksRepository { get; set; }

        [Inject]
        public ISocialLinksRepository SocialLinksRepository { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        public ActionResult Header()
        {
            var headerNavigationLinks = this.NavigationLinksRepository.GetNavigationLinksByPath(Settings.Navigation.HeaderNavigationPath);

            var model = new HeaderViewModel
            {
                HeaderNavigationLinks = this.Mapper.Map<IList<NavigationLinkViewModel>>(headerNavigationLinks),
                CompanyLogoImageUrl = URLHelper.ResolveUrl(Settings.Navigation.CompanyLogoImageUrl),
                CompanyLogoUrl = Settings.Navigation.CompanyLogoLink,
            };

            return PartialView("Header", model);
        }

        public ActionResult Footer()
        {
            var footerNavigationLinks = this.NavigationLinksRepository.GetNavigationLinksByPath(Settings.Navigation.FooterNavigationPath);
            var socialLinks = this.SocialLinksRepository.GetAllSocialIcons();

            var model = new FooterViewModel
            {
                FooterNavigationLinks = this.Mapper.Map<IList<NavigationLinkViewModel>>(footerNavigationLinks),
                SocialLinks = this.Mapper.Map<IList<SocialLinkViewModel>>(socialLinks),
            };

            return PartialView("Footer", model);
        }
    }
}