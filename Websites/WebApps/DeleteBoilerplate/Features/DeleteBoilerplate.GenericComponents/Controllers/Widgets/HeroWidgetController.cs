using System.Web.Mvc;
using AutoMapper;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.HeroWidget;
using Kentico.PageBuilder.Web.Mvc;
using LightInject;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.HeroWidget", typeof(HeroWidgetController),
        "{$DeleteBoilerplate.Widget.Hero.Name$}", Description = "{$DeleteBoilerplate.Widget.Hero.Description$}",
        IconClass = "icon-rectangle-o-h")]

namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class HeroWidgetController : BaseWidgetController<HeroWidgetProperties>
    {
        [Inject]
        public IMapper Mapper { get; set; }

        public ActionResult Index()
        {
            var properties = GetProperties();

            return PartialView("Widgets/_Hero", Mapper.Map<HeroWidgetViewModel>(properties));
        }
    }
}