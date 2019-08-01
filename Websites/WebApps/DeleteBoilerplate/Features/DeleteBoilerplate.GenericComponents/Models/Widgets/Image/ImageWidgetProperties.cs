using System.Collections.Generic;
using DeleteBoilerplate.Infrastructure.Models;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.Image
{
    public class ImageWidgetProperties : BaseWidgetViewModel, IWidgetProperties
    {
        // Assigns a selector component to the 'Images' property
        [EditingComponent(MediaFilesSelector.IDENTIFIER)]
        // Configures the media library from which you can select files in the selector
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.LibraryName), "DeleteBoilerplate")]
        // Limits the maximum number of files that can be selected at once
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.MaxFilesLimit), 5)]
        // Configures the allowed file extensions for the selected files
        [EditingComponentProperty(nameof(MediaFilesSelectorProperties.AllowedExtensions), ".gif;.png;.jpg;.jpeg")]
        // Returns a list of media files selector items (objects that contain the GUIDs of selected media files)
        public IList<MediaFilesSelectorItem> Images { get; set; }
    }
}