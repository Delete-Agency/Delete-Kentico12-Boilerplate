using System.Collections.Generic;
using DeleteBoilerplate.WebApp.Models.Global.Header;

namespace DeleteBoilerplate.WebApp.Models.Global.Footer
{
    public class FooterViewModel
    {
        public IList<NavigationLinkViewModel> FooterNavigationLinks { get; set; }

        public IList<SocialLinkViewModel> SocialLinks { get; set; }
    }
}