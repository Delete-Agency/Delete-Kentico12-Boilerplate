using System.Collections.Generic;

namespace DeleteBoilerplate.WebApp.Models.Global.Header
{
    public class HeaderViewModel
    {
        public IList<NavigationLinkViewModel> HeaderNavigationLinks { get; set; }

        public string CompanyLogoImageUrl { get; set; }

        public string CompanyLogoUrl { get; set; }
    }
}