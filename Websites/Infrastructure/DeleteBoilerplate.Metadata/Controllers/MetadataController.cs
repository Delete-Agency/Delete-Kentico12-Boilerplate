using CMS.DocumentEngine;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.Metadata.Models;
using System.Web.Mvc;

namespace DeleteBoilerplate.Metadata.Controllers
{
    public class MetadataController : BaseController
    {
        public ActionResult RenderMeta()
        {
            var contextItem = this.GetContextItem<TreeNode>();

            var model = this.Mapper.Map<IMetadata>(contextItem);

            return this.PartialView("_Meta", model);
        }
    }
}