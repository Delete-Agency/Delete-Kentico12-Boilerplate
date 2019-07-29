using CMS.DocumentEngine.Types.DeleteBoilerplate;
using DeleteBoilerplate.Infrastructure.Extensions;
using DeleteBoilerplate.Metadata.Models;

namespace DeleteBoilerplate.Metadata.Automap
{
    public class MetadataAutoMap : AutoMapper.Profile
    {
        public MetadataAutoMap()
        {
            CreateMap<IBasePage, MetadataModel>()
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.GetValue("NodeName")))
                .ForMember(dst => dst.CanonicalUrl, opt => opt.MapFrom(src => src.SeoUrl.GetAbsoluteAppUrl()));
        }
    }
}