using System.Web.Mvc;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.DynamicRouting.Attributes;
using DeleteBoilerplate.DynamicRouting.Controllers;

namespace DeleteBoilerplate.WebApp.Controllers
{
    public class HomeController : BaseController
    {
        [PageTypeRouting(Home.CLASS_NAME)]
        public ActionResult Index()
        {
            return View();
        }
    }
}