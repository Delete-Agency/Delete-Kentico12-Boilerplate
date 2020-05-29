using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ImageWidget;
using Kentico.PageBuilder.Web.Mvc;
using System.Web.Mvc;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.ImageWidget", typeof(ImageWidgetController),
        "{$DeleteBoilerplate.Widget.Image.Name$}", Description = "{$DeleteBoilerplate.Widget.Image.Description$}",
        IconClass = "icon-rectangle-o-h")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class ImageWidgetController : BaseWidgetController<ImageWidgetProperties>
    {
        public ActionResult Index()
        {
            var properties = GetProperties();

            return PartialView("Widgets/_Image", Mapper.Map<ImageWidgetViewModel>(properties));
        }
    }
}