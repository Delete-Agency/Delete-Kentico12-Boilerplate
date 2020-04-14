using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
using CMS.Helpers;
using DeleteBoilerplate.Common.Extensions;
using DeleteBoilerplate.Common.Models.Media;
using DeleteBoilerplate.Domain.Services;
using DeleteBoilerplate.Metadata.Models;
using DeleteBoilerplate.Projects.Models;
using DeleteBoilerplate.Projects.Models.Widgets.ProjectsListing;
using DeleteBoilerplate.Projects.Services;
using LightInject;

namespace DeleteBoilerplate.Projects
{
    public class ProjectAutoMap : AutoMapper.Profile
    {
        [Inject]
        protected IProjectDescriber ProjectDescriber { get; set; }

        [Inject]
        protected IUrlSelectorService UrlSelectorService { get; set; }

        public ProjectAutoMap()
        {
            CreateMap<Project, ProjectViewModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.NodeGUID))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => ProjectDescriber.GetDescribe(src)))
                .ForMember(dst => dst.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dst => dst.Url, opt => opt.MapFrom(src => URLHelper.ResolveUrl(src.RelativeURL, false)))
                .ForMember(dst => dst.Image, opt => opt.Ignore())
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

            CreateMap<ProjectsListingWidgetProperties, ProjectsListingWidgetViewModel>(MemberList.None)
                .ForMember(dst => dst.Link, opt => opt.MapFrom(src => UrlSelectorService.GetLink(src.Link)))
                .ForMember(dst => dst.Projects, opt => opt.Ignore());
        }
    }
}