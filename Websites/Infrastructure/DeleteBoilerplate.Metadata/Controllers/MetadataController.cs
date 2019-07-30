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

            if (contextItem is IBasePage basePage)
            {
                var model = this.Mapper.Map<IMetadata>(basePage);

                return this.PartialView("_Meta", model);
            }
            
            return new EmptyResult();
        }
    }
}