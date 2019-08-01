using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.Projects.Controllers.Widgets;
using DeleteBoilerplate.Projects.Models;
using DeleteBoilerplate.Projects.Models.Widgets.ProjectsListing;
using Kentico.PageBuilder.Web.Mvc;
using LightInject;

[assembly:
    RegisterWidget("DeleteBoilerplate.Projects.ProjectsListing", typeof(ProjectsListingWidgetController),
        "Projects Listing", Description = "Projects Listing",
        IconClass = "icon-rectangle-o-h")]


namespace DeleteBoilerplate.Projects.Controllers.Widgets
{
    public class ProjectsListingWidgetController : BaseListingWidgetController<ProjectsListingWidgetProperties, Project>
    {
        [Inject]
        public IMapper Mapper { get; set; }

        public ActionResult Index()
        {
            var properties = GetProperties();

            var pages = properties.GetPages();

            var model = Mapper.Map<List<ProjectViewModel>>(pages);

            return PartialView("Widgets/_ProjectsListing", model);
        }
    }
}