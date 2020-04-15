using DeleteBoilerplate.Infrastructure.Models;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.ContactFormWidget
{
    public class ContactFormWidgetViewModel : BaseWidgetViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ButtonText { get; set; }
    }
}