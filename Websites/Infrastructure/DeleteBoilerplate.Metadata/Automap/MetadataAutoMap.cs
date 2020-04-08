using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Domain;
using DeleteBoilerplate.GenericComponents.Models.Footer;
using DeleteBoilerplate.Infrastructure.Enums;
using DeleteBoilerplate.Infrastructure.Extensions;
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

            CreateMap<NavigationLink, NavigationLinkViewModel>()
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Url, opt => opt.MapFrom(src => URLHelper.ResolveUrl(src.Url, false)))
                .ForMember(dst => dst.HideOnMobile, opt => opt.MapFrom(src => src.HideOnMobile))
                .ForMember(dst => dst.Target, opt => opt.MapFrom(src => src.Target))
                .ForMember(dst => dst.ChildLinks, opt => opt.MapFrom(src => src.ChildLinks))
                .ForMember(dst => dst.AssociatedPage, opt => opt.MapFrom(src => src.AssociatedPage))
                .ForMember(dst => dst.AssociatedPagePath, opt => opt.MapFrom(src => src.AssociatedPagePath))
                .AfterMap((src, dst) =>
                {
                    foreach (var childLink in dst.ChildLinks)
                    {
                        childLink.ParentLink = dst;
                    }
                })
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<SocialIcon, SocialLinkViewModel>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.NodeAlias));
        }
    }
}