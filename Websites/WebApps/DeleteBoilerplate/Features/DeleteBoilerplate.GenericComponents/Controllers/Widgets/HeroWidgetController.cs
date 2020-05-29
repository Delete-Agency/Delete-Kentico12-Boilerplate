using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.HeroWidget;
using Kentico.PageBuilder.Web.Mvc;
using System.Web.Mvc;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.HeroWidget", typeof(HeroWidgetController),
        "{$DeleteBoilerplate.Widget.Hero.Name$}", Description = "{$DeleteBoilerplate.Widget.Hero.Description$}",
        IconClass = "icon-rectangle-o-h")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class HeroWidgetController : BaseWidgetController<HeroWidgetProperties>
    {
        public ActionResult Index()
        {
            var properties = GetProperties();

            return PartialView("Widgets/_Hero", Mapper.Map<HeroWidgetViewModel>(properties));
        }
    }
}