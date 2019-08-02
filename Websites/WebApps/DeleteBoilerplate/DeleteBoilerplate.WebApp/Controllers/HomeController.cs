using System.Web.Mvc;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.DynamicRouting.Attributes;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.OutputCache;

namespace DeleteBoilerplate.WebApp.Controllers
{
    [OutputCache(CacheProfile = OutputCacheConsts.CacheProfiles.Default)]
    public class HomeController : BaseController
    {
        [PageTypeRouting(Home.CLASS_NAME)]
        public ActionResult Index()
        {
            return View();
        }
    }
}