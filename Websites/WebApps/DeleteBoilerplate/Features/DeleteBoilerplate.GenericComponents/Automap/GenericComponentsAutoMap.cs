using DeleteBoilerplate.GenericComponents.Models.Widgets.ContentBlockWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.HeroWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ImageWidget;

namespace DeleteBoilerplate.GenericComponents
{
    public class GenericComponentsAutoMap : AutoMapper.Profile
    {
        public GenericComponentsAutoMap()
        {
            CreateMap<ContentBlockWidgetProperties, ContentBlockWidgetViewModel>();
            CreateMap<ImageWidgetProperties, ImageWidgetViewModel>();
            CreateMap<HeroWidgetProperties, HeroWidgetViewModel>();
        }
    }
}