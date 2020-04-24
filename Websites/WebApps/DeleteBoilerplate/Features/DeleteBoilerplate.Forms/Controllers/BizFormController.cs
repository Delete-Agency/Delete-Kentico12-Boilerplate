using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Forms.Models;
using Kentico.Forms.Web.Mvc;
using LightInject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeleteBoilerplate.Forms.Controllers
{
    public class BizFormController : Controller
    {
        private const string BizFormName = "BizForm";

        [Inject]
        protected IFormProvider FormProvider { get; set; }

        [Inject]
        protected IFormComponentVisibilityEvaluator FormComponentVisibilityEvaluator { get; set; }

        [Inject]
        protected IFormComponentModelBinder FormComponentModelBinder { get; set; }

        [HttpPost]
        public ActionResult Submit(BizFormData formData)
        {
            try
            {
                var formInfo = BizFormInfoProvider.GetBizFormInfo(formData.FormName, SiteContext.CurrentSiteID);
                var formComponents = this.BindFormComponents(formInfo, formData.ElementId);

                bool isValidModel = this.IsValidAndUpdateModelState(formComponents, formData.ElementId);
                if (isValidModel == false)
                {
                    return this.ValidationErrorResult();
                }

                var formItem = this.GetBizFormItem(formInfo, formComponents, formData.ElementId);

                formItem.SubmitChanges(false);
                this.FormProvider.SendEmails(formInfo, formItem);

                return this.SuccessfulResult(formInfo);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException($"{BizFormName} Submit", ex.ToString(), null);

                return this.ServerErrorResult();
            }
        }

        private BizFormItem GetBizFormItem(BizFormInfo formInfo, List<FormComponent> formComponents, string elementId)
        {
             BizFormItem formItem;

            var isExistBizFormItem = BizFormItemProvider.GetItems(DataClassInfoProvider.GetClassName(formInfo.FormClassID))
                .HasExistingItemForContact(formInfo, ContactManagementContext.CurrentContact?.ContactGUID, out formItem);

            formItem = isExistBizFormItem
                ? FormProvider.UpdateFormData(formInfo, formItem.ItemID, formComponents, ContactManagementContext.CurrentContact?.ContactGUID)
                : FormProvider.SetFormData(formInfo, formComponents, ContactManagementContext.CurrentContact?.ContactGUID);

            return formItem;
        }

        private List<FormComponent> BindFormComponents(BizFormInfo formInfo, string elementId)
        {
            var className = DataClassInfoProvider.GetClassName(formInfo.FormClassID);

            var formItems = BizFormItemProvider.GetItems(className);
            var existingFormItem = formItems?.GetExistingItemForContact(formInfo, ContactManagementContext.CurrentContact?.ContactGUID);

            var modelBinder = new FormBuilderModelBinder(formInfo, FormProvider, FormComponentModelBinder, FormComponentVisibilityEvaluator, elementId);
            var modelBindingContext = new FormBuilderModelBindingContext()
            {
                Contact = ContactManagementContext.CurrentContact,
                ExistingItem = existingFormItem
            };

            var formComponents = modelBinder.BindModel(ControllerContext, modelBindingContext) as List<FormComponent>;
            return formComponents;
        }
        
        private bool IsValidAndUpdateModelState(List<FormComponent> formComponents, string elementId)
        {
            this.ModelState.Clear();

            foreach (var item in formComponents)
            {
                this.AddRequiredErrors(item, elementId);

                this.AddRuleErrors(item, elementId);
            }

            bool isValidModel = this.ModelState.Count <= 0 ;
            return isValidModel;
        }

        private void AddRequiredErrors(FormComponent item, string elementId)
        {
            //ToDo: It needs refactoring or thinking
            if (item.BaseProperties.Required)
            {
                string itemTypeName = item.GetType().Name;

                if (itemTypeName.Equals(typeof(IntInputComponent).Name))
                {
                    var val = item.GetObjectValue() as int?;
                    if (val == null)
                        this.AddErrorInModelState(item, elementId, "IntInputComponent");
                }
                else if (itemTypeName.Equals(typeof(TextInputComponent).Name))
                {
                    var val = item.GetObjectValue() as string;
                    if (val.IsNullOrEmpty())
                        this.AddErrorInModelState(item, elementId, "TextInputComponent");
                }
                else if (itemTypeName.Equals(typeof(EmailInputComponent).Name))
                {
                    var val = item.GetObjectValue() as string;
                    if (val.IsNullOrEmpty())
                        this.AddErrorInModelState(item, elementId, "EmailInputComponent");
                }
                else if (itemTypeName.Equals(typeof(CheckBoxComponent).Name))
                {
                    var val = item.GetObjectValue() as bool?;
                    if (val != true)
                        this.AddErrorInModelState(item, elementId, "CheckBoxComponent");
                }
                else if (itemTypeName.Equals(typeof(TextAreaComponent).Name))
                {
                    var val = item.GetObjectValue() as bool?;
                    if (val != true)
                        this.AddErrorInModelState(item, elementId, "TextAreaComponent");
                }
            }
        }

        private void AddRuleErrors(FormComponent item, string elementId)
        {
            foreach (var rule in item.BaseProperties.ValidationRuleConfigurations)
            {
                bool isValid = rule.ValidationRule.IsValueValid(item.GetObjectValue());
                if (isValid == false)
                {
                    this.AddErrorInModelState(item, elementId, rule.ValidationRule.ErrorMessage);
                }
            }
        }

        private void AddErrorInModelState(FormComponent item, string elementId, string errorMessage)
        {
            ModelState.AddModelError($"{elementId}.{item.Name}.{item.LabelForPropertyName}", errorMessage);
        }

        protected virtual ActionResult SuccessfulResult(BizFormInfo formInfo)
        {
            string redirect = formInfo.FormRedirectToUrl ?? string.Empty;
            string displayText = formInfo.FormDisplayText ?? string.Empty;

            return Json(new { Result = true, Redirect = redirect, DisplayText = displayText }, JsonRequestBehavior.AllowGet);
        }

        protected virtual ActionResult ValidationErrorResult()
        {
            this.Response.StatusCode = 422;
            return this.Json(FormResponseModel.BuildValidationErrorResponse(this.ModelState));
        }

        protected virtual ActionResult ServerErrorResult()
        {
            this.Response.StatusCode = 500;

            string errorMessage = ResHelper.GetString("DeleteBoilerplate.Forms.BizFormName.Error");
            return Json(new { Result = false, errorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}