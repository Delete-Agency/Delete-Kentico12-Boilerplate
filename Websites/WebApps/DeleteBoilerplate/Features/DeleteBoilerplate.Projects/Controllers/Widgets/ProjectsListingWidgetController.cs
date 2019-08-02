using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Extensions;
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
    public class ProjectsListingWidgetController : BaseWidgetController<ProjectsListingWidgetProperties>
    {
        [Inject]
        public IMapper Mapper { get; set; }

        public ActionResult Index()
        {
            var properties = GetProperties();
            var projects = properties.GetPages();
            // filter by Taxonomy 
            if (projects.Any())
            {
                var taxonomiesList = properties.ToTaxonomiesList();
                if (taxonomiesList.Any())
                {
                    projects = projects.Where(x => taxonomiesList.Contains(x.Area)).ToList();
                }
            }

            var model = Mapper.Map<List<ProjectViewModel>>(projects);

            return PartialView("Widgets/_ProjectsListing", model);
        }
    }
}