using System.Collections.Generic;
using DeleteBoilerplate.Common.Models;
using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Forms.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.BizFormWidget
{
    public class BizFormWidgetViewModel : BaseWidgetViewModel
    {
        public IList<OptionModel> FormNameOptions { get; set; }

        public string ElementId { get; set; }

        public string FormName { get; set; }

        public FormBuilderConfiguration FormConfiguration { get; set; }

        public IList<FormComponent> FormComponents { get; set; }
    }
}