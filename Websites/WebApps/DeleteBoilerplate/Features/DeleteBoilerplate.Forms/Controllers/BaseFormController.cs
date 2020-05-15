using AutoMapper;
using CMS.Helpers;
using CMS.OnlineForms;
using DeleteBoilerplate.Forms.Models;
using DeleteBoilerplate.Forms.Services;
using DeleteBoilerplate.Infrastructure.Extensions;
using LightInject;
using System.IO;
using System.Text;
using System.Web.Mvc;
using DeleteBoilerplate.Domain;

namespace DeleteBoilerplate.Forms.Controllers
{
    public abstract class BaseFormController<TFormData> : Controller where TFormData : IFormData
    {
        [Inject]
        protected IMapper Mapper { get; set; }

        [Inject]
        protected ICaptchaVerificationService CaptchaVerificationService { get; set; }

        protected virtual ActionResult ProcessForm(TFormData formData)
        {
            if (formData == null)
            {
                var serverErrorResult = new FormResponseModel
                {
                    Type = FormResponseType.ServerError,
                    Message = this.RenderPartialToString("_ServerErrorResult", null)
                };

                return this.Json(serverErrorResult);
            }

            var isVerified = this.CaptchaVerificationService.VerifyCaptcha(this.Request.Form);
            if (isVerified == false)
            {
                
                this.ModelState.AddModelError(this.CaptchaVerificationService.CaptchaHeader, ResHelper.GetStringFormat("DeleteBoilerplate.Forms.CaptchaVerification.Error"));
            }

            return this.ModelState.IsValid
                ? this.ProcessFormInternal(formData)
                : this.ValidationErrorResult();
        }

        protected abstract ActionResult ProcessFormInternal(TFormData formData);

        protected virtual ActionResult ValidationErrorResult()
        {
            this.Response.StatusCode = 422;
            return this.Json(FormResponseModel.BuildValidationErrorResponse(this.ModelState));
        }

        protected virtual void SaveFormData<TFormItem>(TFormData formData) where TFormItem : BizFormItem
        {
            var form = this.Mapper.Map<TFormItem>(formData);
            form.SubmitChanges(false);
        }

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

        /// <summary>
        /// For Ajax usage only
        /// </summary>
        /// <param name="viewName">Partial view name</param>
        /// <param name="model">View model</param>
        /// <returns>Html content of view as string</returns>
        public string RenderPartialToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}