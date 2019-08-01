using System.Collections.Generic;
using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Components.Web.Mvc.FormComponents;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.HeroWidget
{
    public class HeroWidgetViewModel : BaseWidgetViewModel
    {
        public IList<MediaFilesSelectorItem> Images { get; set; }

        public string Text { get; set; }
    }
}