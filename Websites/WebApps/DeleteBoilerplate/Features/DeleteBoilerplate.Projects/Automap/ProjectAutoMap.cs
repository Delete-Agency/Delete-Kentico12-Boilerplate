using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using DeleteBoilerplate.GenericComponents.Models.Widgets;
using DeleteBoilerplate.Infrastructure.Extensions;
using DeleteBoilerplate.Metadata.Models;
using DeleteBoilerplate.Projects.Models;
using DeleteBoilerplate.Projects.Services;
using LightInject;

namespace DeleteBoilerplate.Projects
{
    public class ProjectAutoMap : AutoMapper.Profile
    {
        [Inject]
        public IProjectDescriber ProjectDescriber { get; set; }

        public ProjectAutoMap()
        {
            CreateMap<Project, ProjectViewModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.NodeGUID))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => ProjectDescriber.GetDescribe(src)))
                .ForMember(dst => dst.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dst => dst.ImageUrl, opt => opt.MapFrom(src => src.Image))
                .ForMember(dst => dst.Url, opt => opt.MapFrom(src => URLHelper.ResolveUrl(src.RelativeURL, false)));
                .ForMember(src => src.Image, opt => opt.Ignore())
                .AfterMap((s, d, context) =>
                {
                    if (s.Image != null)
                    {
                        var model = MediaExtensions.GetImageModelByURl(s.Image);
                        if (model != null)
                        {
                            d.Image = context.Mapper.Map<ImageViewModel>(model);
                        }
                    }
                });

            CreateMap<Project, IMetadata>()
                .IncludeBase<TreeNode, IMetadata>();
        }
    }
}