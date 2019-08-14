using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.DynamicRouting.Attributes;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.OutputCache;
using LightInject;

namespace DeleteBoilerplate.WebApp.Controllers
{
    [OutputCache(CacheProfile = OutputCacheConsts.CacheProfiles.Default)]
    public class ContentPageController : BaseController
    {
        [Inject]
        public IMapper Mapper { get; set; }

        [PageTypeRouting(ContentPage.CLASS_NAME)]
        public ActionResult Index()
        {
            var contextItem = this.GetContextItem<ContentPage>();
            //var viewModel = Mapper.Map<ContentPage, ContentPageViewModel>(contextItem);

            return View();
        }
    }
}