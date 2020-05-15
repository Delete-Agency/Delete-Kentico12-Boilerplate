using DeleteBoilerplate.Domain;
using DeleteBoilerplate.Infrastructure.Extensions;
using System.Text;
using System.Web.Mvc;

namespace DeleteBoilerplate.DynamicRouting.Controllers
{
    public abstract class BaseApiController : Controller
    {
        protected JsonResult JsonSuccess(object data = null, string message = null, string contentType = null, Encoding contentEncoding = null, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            var jsonData = new JsonData
            {
                Status = JsonStatus.Success,
                Message = message,
                Data = data
            };

            return this.Json(jsonData, contentType, contentEncoding, behavior);
        }

        protected JsonResult JsonError(string message = null, object data = null, string contentType = null, Encoding contentEncoding = null, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            var jsonData = new JsonData
            {
                Status = JsonStatus.Error,
                Message = message,
                Data = data
            };

            return this.Json(jsonData, contentType, contentEncoding, behavior);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                JsonSerializerSettings = Settings.DefaultJsonSerializerSettings
            };
        }
    }
}