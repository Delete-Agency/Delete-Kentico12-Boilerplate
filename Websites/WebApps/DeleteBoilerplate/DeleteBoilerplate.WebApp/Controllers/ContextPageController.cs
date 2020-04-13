using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.DynamicRouting.Attributes;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.OutputCache;
using System.Web.Mvc;

namespace DeleteBoilerplate.WebApp.Controllers
{
    [OutputCache(CacheProfile = OutputCacheConsts.CacheProfiles.Default)]
    public class ContentPageController : BaseController
    {
        [PageTypeRouting(ContentPage.CLASS_NAME)]
        public ActionResult Index()
        {
            var contextItem = this.GetContextItem<ContentPage>();
            //var viewModel = Mapper.Map<ContentPage, ContentPageViewModel>(contextItem);

            return View();
        }
    }
}