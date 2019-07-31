using DeleteBoilerplate.GenericComponents.Models.Widgets.ContentBlockWidget;

namespace DeleteBoilerplate.GenericComponents
{
    public class GenericComponentsAutoMap : AutoMapper.Profile
    {
        public GenericComponentsAutoMap()
        {
            CreateMap<ContentBlockWidgetProperties, ContentBlockWidgetViewModel>();
        }
    }
}