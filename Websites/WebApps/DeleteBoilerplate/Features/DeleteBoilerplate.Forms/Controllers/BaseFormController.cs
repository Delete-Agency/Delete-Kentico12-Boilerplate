using CMS.Helpers;
using CMS.OnlineForms;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.Forms.Models;
using DeleteBoilerplate.Forms.Services;
using LightInject;
using System.IO;
using System.Web.Mvc;

namespace DeleteBoilerplate.Forms.Controllers
{
    public abstract class BaseFormController<TFormData> : BaseApiController where TFormData : IFormData
    {
        [Inject]
        protected ICaptchaVerificationService CaptchaVerificationService { get; set; }

        protected virtual ActionResult ProcessForm(TFormData formData)
        {
            if (formData == null)
            {
                return JsonError(data: this.RenderPartialToString("_ServerErrorResult", null));
            }

            var isVerified = this.CaptchaVerificationService.VerifyCaptcha(this.Request.Form);
            if (isVerified == false)
            {
                this.ModelState.AddModelError(this.CaptchaVerificationService.CaptchaHeader, ResHelper.GetStringFormat("DeleteBoilerplate.Forms.CaptchaVerification.Error"));
            }

            return this.ModelState.IsValid
                ? this.ProcessFormInternal(formData)
                : JsonValidationError(this.ModelState);
        }

        protected abstract ActionResult ProcessFormInternal(TFormData formData);

        protected virtual void SaveFormData<TFormItem>(TFormData formData) where TFormItem : BizFormItem
        {
            var form = this.Mapper.Map<TFormItem>(formData);
            form.SubmitChanges(false);
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