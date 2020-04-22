using System.Collections.Generic;
using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Forms.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.KenticoFormWidget
{
    public class BizFormWidgetViewModel : BaseWidgetViewModel
    {
        public string ElementId { get; set; }

        public string FormName { get; set; }

        public FormBuilderConfiguration FormConfiguration { get; set; }

        public IList<FormComponent> FormComponents { get; set; }
    }
}