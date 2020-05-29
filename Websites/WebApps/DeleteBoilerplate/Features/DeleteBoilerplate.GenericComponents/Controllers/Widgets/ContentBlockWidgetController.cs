using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ContentBlockWidget;
using Kentico.PageBuilder.Web.Mvc;
using System.Web.Mvc;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.ContentBlockWidget", typeof(ContentBlockWidgetController),
        "{$DeleteBoilerplate.Widget.ContentBlock.Name$}", Description = "{$DeleteBoilerplate.Widget.ContentBlock.Description$}",
        IconClass = "icon-rectangle-o-h")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class ContentBlockWidgetController : BaseWidgetController<ContentBlockWidgetProperties>
    {
        public ActionResult Index()
        {
            var properties = GetProperties();

            return PartialView("Widgets/_ContentBlock", Mapper.Map<ContentBlockWidgetViewModel>(properties));
        }
    }
}