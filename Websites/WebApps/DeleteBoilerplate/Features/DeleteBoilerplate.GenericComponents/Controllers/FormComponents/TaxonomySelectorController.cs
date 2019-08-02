using System.Text;
using System.Web.Mvc;
using DeleteBoilerplate.Domain.Services;
using DeleteBoilerplate.Infrastructure.Extensions;
using LightInject;

namespace DeleteBoilerplate.GenericComponents.Controllers.FormComponents
{
    public class TaxonomySelectorController : Controller
    {
        [Inject]
        public ITaxonomyService TaxonomyService { get; set; }

        public JsonResult GetTaxonomyTree(string targetTaxonomyType)
        {
            var result = TaxonomyService.GetTaxonomyTree(targetTaxonomyType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }

}
