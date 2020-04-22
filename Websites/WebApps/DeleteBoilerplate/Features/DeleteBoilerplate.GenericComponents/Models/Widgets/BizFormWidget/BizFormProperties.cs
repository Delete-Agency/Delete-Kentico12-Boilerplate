using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.BizFormWidget
{
    public class BizFormProperties : BaseWidgetViewModel, IWidgetProperties
    {
        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Form Name", Order = 100, DefaultValue = "Contact")]
        public string FormName { get; set; }
    }
}