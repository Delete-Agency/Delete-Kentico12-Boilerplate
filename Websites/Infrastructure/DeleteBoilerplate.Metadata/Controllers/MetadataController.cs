using System.Web.Mvc;
using AutoMapper;
using CMS.DocumentEngine;
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

            var model = this.Mapper.Map<IMetadata>(contextItem);

            return this.PartialView("_Meta", model);
        }
    }
}