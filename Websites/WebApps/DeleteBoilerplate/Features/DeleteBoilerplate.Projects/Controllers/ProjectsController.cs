using System.Linq;
using System.Net;
using System.Web.Mvc;
using DeleteBoilerplate.Domain.Repositories;
using LightInject;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.DynamicRouting.Attributes;

namespace DeleteBoilerplate.Projects.Controllers
{
    public class ProjectsController : Controller
    {
        [Inject]
        public IProjectRepository ProjectRepository { get; set; }


        [PageTypeRouting(Project.CLASS_NAME)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(int year)
        {
            var projects = ProjectRepository.GetAllProjects().Where(x => x.Year == year).ToList();
            return View("Search",projects.Select(x=>x.Title));
        }
    }
}