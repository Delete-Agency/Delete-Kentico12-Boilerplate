using DeleteBoilerplate.Infrastructure.Models;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.BizFormWidget
{
    public class BizFormProperties : BaseWidgetViewModel, IWidgetProperties
    {
        public string FormName { get; set; }
    }
}