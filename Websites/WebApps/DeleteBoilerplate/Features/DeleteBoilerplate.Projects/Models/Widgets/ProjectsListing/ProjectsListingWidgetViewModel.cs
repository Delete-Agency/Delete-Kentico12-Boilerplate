using System.Collections.Generic;
using DeleteBoilerplate.Common.Models;
using DeleteBoilerplate.Infrastructure.Models;

namespace DeleteBoilerplate.Projects.Models.Widgets.ProjectsListing
{
    public class ProjectsListingWidgetViewModel : BaseWidgetViewModel
    {
        public IList<ProjectViewModel> Projects { get; set; }

        public LinkViewModel Link { get; set; }
    }
}