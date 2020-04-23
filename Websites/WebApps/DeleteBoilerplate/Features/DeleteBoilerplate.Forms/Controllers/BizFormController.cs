using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.SiteProvider;
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
                if (ModelState.IsValid == false)
                {
                    string errorMessage = ResHelper.GetString("DeleteBoilerplate.Forms.BizFormName.InvalidData");
                    return Json(new { Result = false, errorMessage }, JsonRequestBehavior.AllowGet);
                }

                var formInfo = BizFormInfoProvider.GetBizFormInfo(formData.FormName, SiteContext.CurrentSiteID);
                var formItem = this.GetBizFormItem(formInfo, formData.ElementId);

                formItem.SubmitChanges(false);
                this.FormProvider.SendEmails(formInfo, formItem);

                string redirect = formInfo.FormRedirectToUrl ?? string.Empty;
                string displayText = formInfo.FormDisplayText ?? string.Empty;

                return Json(new { Result = true, Redirect = redirect, DisplayText = displayText }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException($"{BizFormName} Submit", ex.ToString(), null);

                string errorMessage = ResHelper.GetString("DeleteBoilerplate.Forms.BizFormName.Error");
                return Json(new { Result = false, errorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        private BizFormItem GetBizFormItem(BizFormInfo formInfo, string elementId)
        {
            var formComponents = this.BindFormComponents(formInfo, elementId);

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
    }
}