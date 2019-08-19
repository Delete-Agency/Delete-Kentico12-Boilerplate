using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.Domain.Services;
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

        [Inject]
        public ITaxonomySearch<Project> TaxonomySearch { get; set; }

        public ActionResult Index(BaseListingFilters filters)
        {
            var properties = GetProperties();

            var taxonomiesList = properties
                .ToTaxonomiesList()
                .Select(Guid.Parse);

            var rootAliasPath = properties.GetRootPageAliasPath();

            var projects = TaxonomySearch.GetItems(taxonomiesList, out _, 0, 10, searchRootAliasPath: rootAliasPath);

            var model = Mapper.Map<List<ProjectViewModel>>(projects);

            return PartialView("Widgets/_ProjectsListing", model);
        }
    }

    public class BaseListingFilters
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}