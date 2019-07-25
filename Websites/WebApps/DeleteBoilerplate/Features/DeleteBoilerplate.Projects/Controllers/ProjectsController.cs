using System.Linq;
using System.Net;
using System.Web.Mvc;
using DeleteBoilerplate.Domain.Repositories;
using LightInject;

namespace DeleteBoilerplate.Projects.Controllers
{
    public class ProjectsController : Controller
    {
        [Inject]
        public IProjectRepository ProjectRepository { get; set; }


        public ActionResult Index()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Search(int year)
        {
            var projects = ProjectRepository.GetAllProjects().Where(x => x.Year == year).ToList();
            return View("Search",projects.Select(x=>x.Title));
        }
    }
}