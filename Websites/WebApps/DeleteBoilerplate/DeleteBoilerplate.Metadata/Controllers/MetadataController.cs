using System.Web.Mvc;
using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.Metadata.Models;
using LightInject;

namespace DeleteBoilerplate.Metadata.Controllers
{
    public class MetadataController : BaseController
    {
        [Inject]
        public IMapper Mapper { get; set; }

        public ActionResult RenderMeta()
        {
            var contextItem = this.GetContextItem<TreeNode>();

            MetadataModel model = null;

            if (contextItem is IBasePage basePage)
                model = this.Mapper.Map<IBasePage, MetadataModel>(basePage);

            return this.PartialView("_Meta", model);
        }
    }
}