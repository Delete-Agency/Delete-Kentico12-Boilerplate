using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.Projects.Models;
using LightInject;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.DynamicRouting.Attributes;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.OutputCache;

namespace DeleteBoilerplate.Projects.Controllers
{
    public class ProjectsController : BaseController
    {
        [Inject]
        public IProjectRepository ProjectRepository { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        [PageTypeRouting(Project.CLASS_NAME)]
        [OutputCache(CacheProfile = OutputCacheConsts.CacheProfiles.Default)]
        public ActionResult Index()
        {
            var contextItem = this.GetContextItem<Project>();
            var viewModel = Mapper.Map<Project, ProjectViewModel>(contextItem);

            return View(viewModel);
        }

        [OutputCache(CacheProfile = OutputCacheConsts.CacheProfiles.Default)]
        public ActionResult Search(int year)
        {
            OutputCacheDependencies.AddPageDependency<Project>();
            var projects = ProjectRepository.GetAllProjects().Where(x => x.Year == year).ToList();
            return View("Search", Mapper.Map<List<ProjectViewModel>>(projects));
        }
    }
}