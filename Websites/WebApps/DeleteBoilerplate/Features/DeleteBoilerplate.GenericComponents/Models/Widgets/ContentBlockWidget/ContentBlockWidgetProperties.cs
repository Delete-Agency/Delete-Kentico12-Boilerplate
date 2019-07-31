using DeleteBoilerplate.Infrastructure.Models;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.ContentBlockWidget
{
    public class ContentBlockWidgetProperties : BaseWidgetViewModel, IWidgetProperties
    {
        public string Text { get; set; }
    }
}