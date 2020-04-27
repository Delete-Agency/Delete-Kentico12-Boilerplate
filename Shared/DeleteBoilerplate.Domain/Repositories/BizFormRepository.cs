using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.OnlineForms;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Models;
using Kentico.Forms.Web.Mvc;
using LightInject;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DeleteBoilerplate.Domain.Repositories
{
    public interface IBizFormRepository
    {
        IList<BizFormInfo> GetSiteForms();

        IList<OptionModel> GetSiteFormNames();

        BizFormInfo GetFormInfo(string formName);

        ObjectQuery<BizFormItem> GetQueryFormItem(int formClassId);

        BizFormItem GetExistingFormItemForContact(BizFormInfo formInfo, ContactInfo contactInfo);

        IList<FormComponent> GetFormComponentsMappedToContact(BizFormInfo formInfo, ControllerContext controllerContext,
            ContactInfo contactInfo, string elementId);

        IList<FormComponent> GetDisplayedFormComponents(BizFormInfo formInfo, ContactInfo contactInfo);
    }

    public class BizFormRepository : IBizFormRepository
    {
        [Inject]
        protected IFormProvider FormProvider { get; set; }

        [Inject]
        protected IFormComponentVisibilityEvaluator FormComponentVisibilityEvaluator { get; set; }

        [Inject]
        protected IFormComponentModelBinder FormComponentModelBinder { get; set; }

        public IList<BizFormInfo> GetSiteForms()
        {
             var siteForms  = BizFormInfoProvider
                .GetBizForms()
                .Columns("FormSiteID", "FormName", "FormDisplayName", "FormDevelopmentModel")
                .WhereEquals("FormDevelopmentModel", 1)
                .WhereEquals("FormSiteID", SiteContext.CurrentSiteID)
                .ToList();

             return siteForms;
        }

        public IList<OptionModel> GetSiteFormNames()
        {
            var siteFormNames = this.GetSiteForms()
                .Select(x => new OptionModel{
                    Name = x.FormDisplayName,
                    Value = x.FormName
                }).ToList();

            return siteFormNames;
        }

        public BizFormInfo GetFormInfo(string formName)
        {
            var formInfo = BizFormInfoProvider.GetBizFormInfo(formName, SiteContext.CurrentSiteID);
            return formInfo;
        }

        public ObjectQuery<BizFormItem> GetQueryFormItem(int formClassId)
        {
            var formClassName = DataClassInfoProvider.GetClassName(formClassId);

            var queryFormItem = BizFormItemProvider.GetItems(formClassName);
            return queryFormItem;
        }

        public BizFormItem GetExistingFormItemForContact(BizFormInfo formInfo, ContactInfo contactInfo)
        {
            var queryFormItems = this.GetQueryFormItem(formInfo.FormClassID);

            var existingFormItemForContact = queryFormItems?.GetExistingItemForContact(formInfo, contactInfo?.ContactGUID);
            return existingFormItemForContact;
        }

        public IList<FormComponent> GetFormComponentsMappedToContact(BizFormInfo formInfo, ControllerContext controllerContext, ContactInfo contactInfo, string elementId)
        {
            var existingFormItemForContact = this.GetExistingFormItemForContact(formInfo, contactInfo);
            var bindingContextContact = new FormBuilderModelBindingContext
            {
                Contact = contactInfo,
                ExistingItem = existingFormItemForContact
            };

            var modelBinder = new FormBuilderModelBinder(formInfo, FormProvider, FormComponentModelBinder, FormComponentVisibilityEvaluator, elementId);

            var formComponents = modelBinder.BindModel(controllerContext, bindingContextContact) as List<FormComponent>;
            return formComponents;
        }

        public IList<FormComponent> GetDisplayedFormComponents(BizFormInfo formInfo, ContactInfo contactInfo)
        {
            var formItem = this.GetExistingFormItemForContact(formInfo, contactInfo);

            var displayedFormComponents = this.FormProvider
                .GetFormComponents(formInfo)
                .GetDisplayedComponents(contactInfo, formInfo, formItem, this.FormComponentVisibilityEvaluator);

            return displayedFormComponents.ToList();
        }
    }
}
