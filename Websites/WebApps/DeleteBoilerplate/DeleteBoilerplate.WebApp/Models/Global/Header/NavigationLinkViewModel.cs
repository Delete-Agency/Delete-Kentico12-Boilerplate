using System;
using System.Collections.Generic;

namespace DeleteBoilerplate.WebApp.Models.Global.Header
{
    public class NavigationLinkViewModel
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Target { get; set; }

        public Guid AssociatedPage { get; set; }

        public string AssociatedPagePath { get; set; }

        public bool IsActive { get; set; }

        public string ActiveColor { get; set; }

        public bool HideOnMobile { get; set; }

        public NavigationLinkViewModel ParentLink { get; set; }

        public IEnumerable<NavigationLinkViewModel> ChildLinks { get; set; }
    }
}