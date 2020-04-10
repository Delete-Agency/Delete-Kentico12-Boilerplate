using AutoMapper;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using DeleteBoilerplate.WebApp.Models.Global.Footer;
using DeleteBoilerplate.WebApp.Models.Global.Header;

namespace DeleteBoilerplate.WebApp
{
    public class WebAppAutoMap : AutoMapper.Profile
    {
        public WebAppAutoMap()
        {
            CreateMap<NavigationLink, NavigationLinkViewModel>(MemberList.None)
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Url, opt => opt.MapFrom(src => URLHelper.ResolveUrl(src.Url, false)))
                .ForMember(dst => dst.HideOnMobile, opt => opt.MapFrom(src => src.HideOnMobile))
                .ForMember(dst => dst.Target, opt => opt.MapFrom(src => src.Target))
                .ForMember(dst => dst.ActiveColor, opt => opt.MapFrom(src => src.ActiveColor))
                .ForMember(dst => dst.ChildLinks, opt => opt.MapFrom(src => src.ChildLinks))
                .ForMember(dst => dst.AssociatedPage, opt => opt.MapFrom(src => src.AssociatedPage))
                .ForMember(dst => dst.AssociatedPagePath, opt => opt.MapFrom(src => src.AssociatedPagePath))
                .AfterMap((src, dst) =>
                {
                    foreach (var childLink in dst.ChildLinks)
                    {
                        childLink.ParentLink = dst;
                    }
                });

            CreateMap<SocialIcon, SocialLinkViewModel>(MemberList.None)
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.NodeAlias))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dst => dst.Image, opt => opt.MapFrom(src => src.Image));
        }
    }
}