using System.Web.Mvc;
using AutoMapper;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ContentBlockWidget;
using Kentico.PageBuilder.Web.Mvc;
using LightInject;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.ContentBlockWidget", typeof(ContentBlockWidgetController),
        "{$DeleteBoilerplate.Widget.ContentBlock.Name$}", Description = "{$DeleteBoilerplate.Widget.ContentBlock.Description$}",
        IconClass = "icon-rectangle-o-h")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class ContentBlockWidgetController : BaseWidgetController<ContentBlockWidgetProperties>
    {
        [Inject]
        public IMapper Mapper { get; set; }

        public ActionResult Index()
        {
            var properties = GetProperties();

            return PartialView("Widgets/_ContentBlock", Mapper.Map<ContentBlockWidgetViewModel>(properties));
        }
    }
}