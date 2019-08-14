using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeleteBoilerplate.GenericComponents.Models.Footer;

namespace DeleteBoilerplate.GenericComponents.Models.Header
{
    public class HeaderViewModel
    {
        public string NavBackgroundImage { get; set; }

        public string NavBackgroundImageAlt { get; set; }

        public List<NavigationLinkViewModel> NavLinks { get; set; }

        public List<SocialLinkViewModel> SocLinks { get; set; }

        public NavigationLinkViewModel ActiveLink { get; set; }

        public int DefaultViewItemsNumber { get; set; }

        public int ExpandedViewItemsNumber { get; set; }

        public IEnumerable<NavigationLinkViewModel> GetSecondLevelLinks()
        {
            var result = new List<NavigationLinkViewModel>();

            if (ActiveLink != null)
            {
                result.Add(ActiveLink.ParentLink ?? ActiveLink);

                result.AddRange(ActiveLink.ParentLink == null
                    ? ActiveLink.ChildLinks
                    : ActiveLink.ParentLink.ChildLinks);
            }

            return result;
        }
    }
}