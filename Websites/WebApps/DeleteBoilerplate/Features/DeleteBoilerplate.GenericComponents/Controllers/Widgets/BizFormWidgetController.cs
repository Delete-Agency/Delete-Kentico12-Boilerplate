using AutoMapper;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.OnlineForms;
using CMS.SiteProvider;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.BizFormWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.KenticoFormWidget;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using LightInject;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.BizFormWidget", typeof(BizFormWidgetController),
        "{$DeleteBoilerplate.GenericComponents.BizFormWidget.Name$}", Description = "{$DeleteBoilerplate.GenericComponents.BizFormWidget.Description$}",
        IconClass = "icon-form")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class BizFormWidgetController : BaseWidgetController<BizFormProperties>
    {
        private const string BizFormName = "BizForm";

        [Inject]
        protected IMapper Mapper { get; set; }

        [Inject]
        protected IFormProvider FormProvider { get; set; }

        [Inject]
        protected IFormComponentVisibilityEvaluator FormComponentVisibilityEvaluator { get; set; }

        public ActionResult Index()
        {
            try
            {
                var properties = GetProperties();

                var formInfo = BizFormInfoProvider.GetBizFormInfo(properties.FormName, SiteContext.CurrentSiteName);
                if (formInfo == null)
                {
                    return null;
                }

                var formComponents = this.GetFormComponents(formInfo);
                var formConfiguration = this.GetFormBuilderConfiguration(formInfo.FormBuilderLayout);

                var model = new BizFormWidgetViewModel()
                {
                    ElementId = formInfo.FormID.ToString(),
                    FormName = formInfo.FormName,
                    FormComponents = formComponents,
                    FormConfiguration = formConfiguration
                };

                return PartialView("Widgets/_BizForm", model);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException($"{BizFormName} Index", ex.ToString(), null);
                return null;
            }
        }

        private IList<FormComponent> GetFormComponents(BizFormInfo formInfo)
        {
            string className = DataClassInfoProvider.GetClassName(formInfo.FormClassID);

            var formItems = BizFormItemProvider.GetItems(className);
            var existingFormItem = formItems?.GetExistingItemForContact(formInfo, ContactManagementContext.CurrentContact?.ContactGUID);

            var formComponents = FormProvider
                .GetFormComponents(formInfo)
                .GetDisplayedComponents(ContactManagementContext.CurrentContact, formInfo, existingFormItem, this.FormComponentVisibilityEvaluator);

            IList<FormComponent> validFormComponents = new List<FormComponent>();
            foreach (var formComponent in formComponents)
            {
                formComponent.Name = $"{formInfo.FormID}.{formComponent.Name}";
                validFormComponents.Add(formComponent);
            }

            return validFormComponents;
        }

        private FormBuilderConfiguration GetFormBuilderConfiguration(string formBuilderLayout)
        {
            var formDeserializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };

            var formConfiguration = JsonConvert.DeserializeObject<FormBuilderConfiguration>(formBuilderLayout, formDeserializationSettings);
            return formConfiguration;
        }
    }
}