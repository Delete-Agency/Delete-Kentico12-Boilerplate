using System.Linq;
using AutoMapper;
using DeleteBoilerplate.Common.Models.Media;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ContactFormWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ContentBlockWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.HeroWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ImageWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.StaticHtmlWidget;
using DeleteBoilerplate.Infrastructure.Extensions;

namespace DeleteBoilerplate.GenericComponents
{
    public class GenericComponentsAutoMap : AutoMapper.Profile
    {
        public GenericComponentsAutoMap()
        {
            CreateMap<ContentBlockWidgetProperties, ContentBlockWidgetViewModel>();
            CreateMap<HeroWidgetProperties, HeroWidgetViewModel>();

            CreateMap<ImageWidgetProperties, ImageWidgetViewModel>()
                .ForMember(src => src.Images, opt => opt.Ignore())
                .AfterMap((s, d, context) =>
                {
                    if (s.Images != null)
                    {
                        d.Images = s.Images.Select(x=>
                        {
                            var model = x.GetImageModel();
                            if (model != null)
                            {
                                return context.Mapper.Map<ImageViewModel>(model);
                            }
                            return null;
                        }).Where(x=>x!=null).ToList();
                    }
                });

            CreateMap<StaticHtmlWidgetProperties, StaticHtmlWidgetViewModel>()
                .ForMember(dst => dst.Html, opt => opt.MapFrom(src => src.Html));

            CreateMap<ContactFormProperties, ContactFormWidgetViewModel>(MemberList.None)
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(d => d.ButtonText, opt => opt.MapFrom(src => src.ButtonText));
        }
    }
}