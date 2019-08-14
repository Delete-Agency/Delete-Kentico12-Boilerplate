
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeleteBoilerplate.GenericComponents.Models.Footer
{
    public class FooterViewModel
    {
        public List<NavigationLinkViewModel> NavigationLinks { get; set; }

        public List<SocialLinkViewModel> SocialLinks { get; set; }
    }
}