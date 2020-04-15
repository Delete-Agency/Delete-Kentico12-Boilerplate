using AutoMapper;
using CMS.Helpers;
using DeleteBoilerplate.Forms.Models;
using DeleteBoilerplate.Forms.Services;
using DeleteBoilerplate.Infrastructure.Extensions;
using LightInject;
using System.Text;
using System.Web.Mvc;
using CMS.OnlineForms;

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
                    Message = ResHelper.GetStringFormat("DeleteBoilerplate.Forms.Data.Error")
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