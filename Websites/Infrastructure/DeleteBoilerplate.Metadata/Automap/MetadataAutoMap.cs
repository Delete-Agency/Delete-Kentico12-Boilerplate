using AutoMapper;
using CMS.DocumentEngine;
using CMS.Localization;
using CMS.SiteProvider;
using DeleteBoilerplate.Infrastructure.Constants;
using DeleteBoilerplate.Infrastructure.Enums;
using DeleteBoilerplate.Infrastructure.Extensions;
using DeleteBoilerplate.Infrastructure.Helpers;
using DeleteBoilerplate.Metadata.Models;

namespace DeleteBoilerplate.Metadata
{
    public class MetadataAutoMap : AutoMapper.Profile
    {
        public MetadataAutoMap()
        {
            CreateMap<TreeNode, IMetadata>(MemberList.None)
                //Metadata mapping
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.DocumentPageTitle))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.DocumentPageDescription))
                .ForMember(dst => dst.CanonicalUrl, opt => opt.MapFrom(src => src.GetAbsoluteUrl()))
                //Open Graph metadata mapping
                .ForPath(dst => dst.OpenGraphMetadata.Type, opt => opt.MapFrom(src => OpenGraphType.Website))
                .ForPath(dst => dst.OpenGraphMetadata.Image,
                    opt => opt.MapFrom(src => Settings.Global.DefaultImage.GetAbsoluteUrl()))
                .ForPath(dst => dst.OpenGraphMetadata.ImageAlt,
                    opt => opt.MapFrom(src => Settings.Global.DefaultImageAlt))
                .ForPath(dst => dst.OpenGraphMetadata.SiteName, opt => opt.MapFrom(src => SiteContext.CurrentSiteName))
                .ForPath(dst => dst.OpenGraphMetadata.Locale,
                    opt => opt.MapFrom(src => LocalizationContext.GetCurrentUICulture().CultureCode.Replace("-", "_")))
                .ForPath(dst => dst.OpenGraphMetadata.Url, opt => opt.MapFrom(src => src.GetAbsoluteUrl()))
                .ForPath(dst => dst.OpenGraphMetadata.Title, opt => opt.MapFrom(src => src.DocumentPageTitle))
                .ForPath(dst => dst.OpenGraphMetadata.Description, opt => opt.MapFrom(src => src.DocumentPageDescription));
        }
    }
}