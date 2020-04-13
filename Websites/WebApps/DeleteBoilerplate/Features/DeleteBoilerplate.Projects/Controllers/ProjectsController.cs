using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.Domain.Services;
using DeleteBoilerplate.DynamicRouting.Attributes;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.OutputCache;
using DeleteBoilerplate.Projects.Models;
using LightInject;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DeleteBoilerplate.Projects.Controllers
{
    [OutputCache(CacheProfile = OutputCacheConsts.CacheProfiles.Default)]
    public class ProjectsController : BaseController
    {
        [Inject]
        public IProjectRepository ProjectRepository { get; set; }

        [Inject]
        public ITaxonomySearch<Project> TaxonomySearch { get; set; }

        [PageTypeRouting(Project.CLASS_NAME)]
        public ActionResult Index()
        {
            var contextItem = this.GetContextItem<Project>();
            var viewModel = Mapper.Map<Project, ProjectViewModel>(contextItem);

            return View(viewModel);
        }

        public ActionResult Search(int year)
        {
            OutputCacheDependencies.AddPageDependency<Project>();
            var projects = ProjectRepository.GetAllProjects().Where(x => x.Year == year).ToList();
            return View("Search", Mapper.Map<List<ProjectViewModel>>(projects));
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult SearchByArea(string area)
        {
            OutputCacheDependencies.AddPageDependency<Project>();
            var projects = TaxonomySearch.GetItems(new TaxonomyExtractBuilder().AddTaxonomyItems("area", area));
            return View("Search", Mapper.Map<List<ProjectViewModel>>(projects));
        }
    }
}