using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.ContactFormWidget
{
    public class ContactFormProperties : BaseWidgetViewModel, IWidgetProperties
    {
        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Title", Order = 100, DefaultValue = "Contact")]
        public string Title { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "Description", Order = 200)]
        public string Description { get; set; }

        [EditingComponent(TextInputComponent.IDENTIFIER, Label = "ButtonText", Order = 300, DefaultValue = "Get in touch")]
        public string ButtonText { get; set; }
    }
}