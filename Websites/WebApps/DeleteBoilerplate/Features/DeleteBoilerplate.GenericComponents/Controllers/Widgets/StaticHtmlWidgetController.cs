using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DeleteBoilerplate.Domain.Repositories;
using DeleteBoilerplate.DynamicRouting.Controllers;
using DeleteBoilerplate.GenericComponents.Controllers.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.StaticHtmlWidget;
using Kentico.PageBuilder.Web.Mvc;
using LightInject;

[assembly:
    RegisterWidget("DeleteBoilerplate.GenericComponents.StaticHtmlWidget", typeof(StaticHtmlWidgetController),
        "{$DeleteBoilerplate.Widget.StaticHtml.Name$}", Description = "{$DeleteBoilerplate.Widget.StaticHtml.Description$}",
        IconClass = "icon-xml-tag")]
namespace DeleteBoilerplate.GenericComponents.Controllers.Widgets
{
    public class StaticHtmlWidgetController : BaseWidgetController<StaticHtmlWidgetProperties>
    {
        [Inject]
        public IMapper Mapper { get; set; }

        [Inject]
        protected IStaticHtmlChunkRepository StaticHtmlChunkRepository { get; set; }

        protected StaticHtmlWidgetViewModel GetModel(StaticHtmlWidgetProperties properties)
        {
            var model = this.Mapper.Map<StaticHtmlWidgetViewModel>(properties);

            if (string.IsNullOrWhiteSpace(model.Html))
            {
                var selectedItem = properties.StaticHtmlChunks?.FirstOrDefault();
                if (selectedItem != null)
                    model.Html = this.StaticHtmlChunkRepository.GetByNodeGuid(selectedItem.NodeGuid).Html;
            }

            return model;
        }

        public ActionResult Index()
        {
            var properties = GetProperties();

            return PartialView("Widgets/_StaticHtml", GetModel(properties));
        }
    }
}