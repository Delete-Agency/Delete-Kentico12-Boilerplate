using System.Net;
using System.Web.Mvc;

namespace DeleteBoilerplate.Projects.Controllers
{
    public class ProjectsController : Controller
    {
        public ActionResult Index()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}