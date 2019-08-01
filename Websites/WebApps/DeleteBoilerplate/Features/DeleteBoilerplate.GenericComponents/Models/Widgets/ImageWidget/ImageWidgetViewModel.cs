using System.Collections.Generic;
using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Components.Web.Mvc.FormComponents;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.ImageWidget
{
    public class ImageWidgetViewModel : BaseWidgetViewModel
    {
        public IList<MediaFilesSelectorItem> Images { get; set; }
    }
}