using AutoMapper;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.SiteProvider;
using DeleteBoilerplate.Infrastructure.Enums;
using DeleteBoilerplate.Infrastructure.Extensions;
using DeleteBoilerplate.Infrastructure.Helpers;
using DeleteBoilerplate.Metadata.Models;

namespace DeleteBoilerplate.Metadata.Automap
{
    public class MetadataAutoMap : AutoMapper.Profile
    {
        public MetadataAutoMap()
        {
            CreateMap<IBasePage, IMetadata>(MemberList.None)
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.MetadataTitle))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.MetadataDescription))
                .ForMember(dst => dst.CanonicalUrl, opt => opt.MapFrom(src => src.SeoUrl.GetAbsoluteAppUrl()))

                .ForPath(dst => dst.OpenGraphMetadata.Type, opt => opt.MapFrom(src => OpenGraphType.Website))
                .ForPath(dst => dst.OpenGraphMetadata.Image,
                    opt => opt.MapFrom(src => SiteSettingsHelper.GetSettingValue(SiteSetting.DefaultImage).GetAbsoluteAppUrl()))
                .ForPath(dst => dst.OpenGraphMetadata.ImageAlt,
                    opt => opt.MapFrom(src => SiteSettingsHelper.GetSettingValue(SiteSetting.DefaultImageAlt)))
                .ForPath(dst => dst.OpenGraphMetadata.SiteName, opt => opt.MapFrom(src => SiteContext.CurrentSiteName))
                .ForPath(dst => dst.OpenGraphMetadata.Url, opt => opt.MapFrom(src => src.SeoUrl))
                .ForPath(dst => dst.OpenGraphMetadata.Title, opt => opt.MapFrom(src => src.MetadataTitle))
                .ForPath(dst => dst.OpenGraphMetadata.Description, opt => opt.MapFrom(src => src.MetadataDescription));
        }
    }
}