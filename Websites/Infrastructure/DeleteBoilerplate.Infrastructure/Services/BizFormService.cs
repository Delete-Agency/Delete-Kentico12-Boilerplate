using CMS.ContactManagement;
using CMS.OnlineForms;
using DeleteBoilerplate.Domain.Repositories;
using Kentico.Forms.Web.Mvc;
using LightInject;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Services
{
    public interface IBizFormService
    {
        BizFormItem GetExistingFormItemForContact(BizFormInfo formInfo, ContactInfo contactInfo);

        IList<FormComponent> GetFormComponentsMappedToContact(BizFormInfo formInfo, ControllerContext controllerContext, ContactInfo contactInfo, string elementId);

        IList<FormComponent> GetDisplayedFormComponents(BizFormInfo formInfo, ContactInfo contactInfo);
    }

    public class BizFormService : IBizFormService
    {
        [Inject]
        protected IFormProvider FormProvider { get; set; }

        [Inject]
        protected IFormComponentVisibilityEvaluator FormComponentVisibilityEvaluator { get; set; }

        [Inject]
        protected IFormComponentModelBinder FormComponentModelBinder { get; set; }

        [Inject]
        protected IBizFormRepository BizFormRepository { get; set; }

        public BizFormItem GetExistingFormItemForContact(BizFormInfo formInfo, ContactInfo contactInfo)
        {
            var queryFormItems = this.BizFormRepository.GetQueryFormItem(formInfo.FormClassID);

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

            var modelBinder = new FormBuilderModelBinder(formInfo, this.FormProvider, this.FormComponentModelBinder, this.FormComponentVisibilityEvaluator, elementId);

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