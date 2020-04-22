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
                var formInfo = BizFormInfoProvider.GetBizFormInfo(formData.FormName, SiteContext.CurrentSiteID);
                var className = DataClassInfoProvider.GetClassName(formInfo.FormClassID);

                var formComponents = this.BindFormComponents(formInfo, className, formData.ElementId);

                if (ModelState.IsValid == false)
                {
                    string errorMessage = ResHelper.GetString("DeleteBoilerplate.Forms.BizFormName.InvalidData");
                    return Json(new { Result = false, errorMessage }, JsonRequestBehavior.AllowGet);
                }

                this.SaveFormData(formInfo, formComponents);

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException($"{BizFormName} Submit", ex.ToString(), null);

                string errorMessage = ResHelper.GetString("DeleteBoilerplate.Forms.BizFormName.Error");
                return Json(new { Result = false, errorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<FormComponent> BindFormComponents(BizFormInfo formInfo, string className, string elementId)
        {
            var items = BizFormItemProvider.GetItems(className);
            var existingItem = items?.GetExistingItemForContact(formInfo, ContactManagementContext.CurrentContact?.ContactGUID);

            var modelBinder = new FormBuilderModelBinder(formInfo, FormProvider, FormComponentModelBinder, FormComponentVisibilityEvaluator, elementId);
            var modelBindingContext = new FormBuilderModelBindingContext()
            {
                Contact = ContactManagementContext.CurrentContact,
                ExistingItem = existingItem
            };

            var formComponents = modelBinder.BindModel(ControllerContext, modelBindingContext) as List<FormComponent>;
            return formComponents;
        }

        private void SaveFormData(BizFormInfo formInfo, List<FormComponent> formComponents)
        {
            BizFormItem bizFormItem;

            var isExistBizFormItem = BizFormItemProvider.GetItems(DataClassInfoProvider.GetClassName(formInfo.FormClassID))
                .HasExistingItemForContact(formInfo, ContactManagementContext.CurrentContact?.ContactGUID, out bizFormItem);

            bizFormItem = isExistBizFormItem 
                ? FormProvider.UpdateFormData(formInfo, bizFormItem.ItemID, formComponents, ContactManagementContext.CurrentContact?.ContactGUID) 
                : FormProvider.SetFormData(formInfo, formComponents, ContactManagementContext.CurrentContact?.ContactGUID);

            bizFormItem.SubmitChanges(false);
        }
    }
}