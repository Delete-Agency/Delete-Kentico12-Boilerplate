using System.Web.Mvc;
using DeleteBoilerplate.DynamicRouting.Helpers;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Web.Mvc;

namespace DeleteBoilerplate.DynamicRouting.Controllers
{
    /// <summary>
    /// This class handles Page Templates for pages that are using the Dynamic Routing
    /// </summary>
    public class DynamicPageTemplateController : PageTemplateController
    {
        // GET: DynamicPageTemplate, finds the node based on the current request url and then renders the template result
        public ActionResult Index()
        {
            var foundNode = DocumentQueryHelper.GetNodeByAliasPath(EnvironmentHelper.GetUrl(HttpContext.Request));
            if (foundNode != null)
            {
                HttpContext.Kentico().PageBuilder().Initialize(foundNode.DocumentID);
                return new TemplateResult(foundNode.DocumentID);
            }

            return new HttpNotFoundResult();
        }

        public ActionResult NotFound()
        {
            return new HttpNotFoundResult("No template selected for this page.");
        }

        public ActionResult UnregisteredTemplate()
        {
            var foundNode = DocumentQueryHelper.GetNodeByAliasPath(EnvironmentHelper.GetUrl(HttpContext.Request));
            if (foundNode != null)
            {
                HttpContext.Kentico().PageBuilder().Initialize(foundNode.DocumentID);
                return View();
            }

            return new HttpNotFoundResult();

        }
    }
}