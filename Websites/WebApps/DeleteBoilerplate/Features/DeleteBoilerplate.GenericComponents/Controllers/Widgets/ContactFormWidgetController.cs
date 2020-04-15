using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using Kentico.PageBuilder.Web.Mvc;
using System.Web.Mvc;
using AutoMapper;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ContactFormWidget;
using LightInject;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.ContactFormWidget", typeof(ContactFormWidgetController),
    "{$DeleteBoilerplate.GenericComponents.ContactFormWidget.Name$}", Description = "{$DeleteBoilerplate.GenericComponents.ContactFormWidget.Description$}",
    IconClass = "icon-form")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class ContactFormWidgetController : BaseWidgetController<ContactFormProperties>
    {
        [Inject]
        protected IMapper Mapper { get; set; }

        public ActionResult Index()
        {
            var properties = GetProperties();
            var model = this.Mapper.Map<ContactFormWidgetViewModel>(properties);

            return PartialView("Widgets/_ContactForm", model);
        }
    }
}