using System.Collections.Generic;
using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.Image
{
    public class ImageWidgetProperties : BaseWidgetViewModel, IWidgetProperties
    {
        public IList<MediaFilesSelectorItem> Images { get; set; }
    }
}