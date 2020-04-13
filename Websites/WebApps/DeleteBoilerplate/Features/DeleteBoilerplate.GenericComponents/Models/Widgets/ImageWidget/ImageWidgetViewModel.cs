using System.Collections.Generic;
using DeleteBoilerplate.Common.Models.Media;
using DeleteBoilerplate.Infrastructure.Models;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.ImageWidget
{
    public class ImageWidgetViewModel : BaseWidgetViewModel
    {
        public IList<ImageViewModel> Images { get; set; }
    }
}