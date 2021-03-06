﻿using System.Linq;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Localization;
using CMS.SiteProvider;
using DeleteBoilerplate.GenericComponents.Models.Widgets;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ContentBlockWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.HeroWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ImageWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.StaticHtmlWidget;
using DeleteBoilerplate.Infrastructure.Extensions;
using DeleteBoilerplate.Infrastructure.Models.Media;

namespace DeleteBoilerplate.GenericComponents
{
    public class GenericComponentsAutoMap : AutoMapper.Profile
    {
        public GenericComponentsAutoMap()
        {
            CreateMap<ImageModel, ImageViewModel>();

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
        }
    }
}