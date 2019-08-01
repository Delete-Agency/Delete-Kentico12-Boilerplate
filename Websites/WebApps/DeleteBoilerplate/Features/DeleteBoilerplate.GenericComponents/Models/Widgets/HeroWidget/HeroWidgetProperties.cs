using System.Collections.Generic;
using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.HeroWidget
{
    public class HeroWidgetProperties : BaseWidgetViewModel, IWidgetProperties
    {
        public IList<MediaFilesSelectorItem> Images { get; set; }

        public string Text { get; set; }
    }
}