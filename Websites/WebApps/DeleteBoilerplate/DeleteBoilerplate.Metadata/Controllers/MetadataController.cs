using System.Web.Mvc;
using CMS.DocumentEngine;
using DeleteBoilerplate.DynamicRouting.Controllers;

namespace DeleteBoilerplate.Metadata.Controllers
{
    public class MetadataController : BaseController
    {
        public ActionResult RenderMeta()
        {
            var contextItem = this.GetContextItem<TreeNode>();

            return this.PartialView("_Meta", contextItem);
        }
    }
}