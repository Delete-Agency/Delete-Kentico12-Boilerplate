using CMS.ContactManagement;
using CMS.OnlineForms;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Helpers;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.BizFormWidget;
using DeleteBoilerplate.Infrastructure.Services;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using LightInject;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.BizFormWidget", typeof(BizFormWidgetController),
        "{$DeleteBoilerplate.GenericComponents.BizFormWidget.Name$}", Description = "{$DeleteBoilerplate.GenericComponents.BizFormWidget.Description$}",
        IconClass = "icon-form")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class BizFormWidgetController : BaseWidgetController<BizFormProperties>
    {
        [Inject]
        protected IBizFormRepository BizFormRepository { get; set; }

        [Inject]
        protected IBizFormService BizFormService { get; set; }

        public ActionResult Index()
        {
            try
            {
                var properties = GetProperties();

                var formNameOptions = this.BizFormRepository.GetSiteFormNames();

                var formInfo = BizFormInfoProvider.GetBizFormInfo(properties.FormName, SiteContext.CurrentSiteName);
                if (formInfo == null)
                {
                    return PartialView("Widgets/_BizForm", new BizFormWidgetViewModel
                    {
                        FormNameOptions = formNameOptions
                    });
                }

                var formComponents = this.GetFormComponents(formInfo);
                var formConfiguration = this.GetFormBuilderConfiguration(formInfo.FormBuilderLayout);

                var model = new BizFormWidgetViewModel
                {
                    ElementId = formInfo.FormID.ToString(),
                    FormName = formInfo.FormName,
                    FormComponents = formComponents,
                    FormConfiguration = formConfiguration,
                    FormNameOptions = formNameOptions
                };

                return PartialView("Widgets/_BizForm", model);
            }
            catch (Exception ex)
            {
                LogHelper.LogException(ex);
                return null;
            }
        }

        private IList<FormComponent> GetFormComponents(BizFormInfo formInfo)
        {
            var currentContact = ContactManagementContext.CurrentContact;

            var displayedFormComponents = this.BizFormService.GetDisplayedFormComponents(formInfo, currentContact);

            var elementId = formInfo.FormID;

            displayedFormComponents.ForEach(x =>
            {
                x.Name = $"{elementId}.{x.Name}";
            });

            return displayedFormComponents;
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