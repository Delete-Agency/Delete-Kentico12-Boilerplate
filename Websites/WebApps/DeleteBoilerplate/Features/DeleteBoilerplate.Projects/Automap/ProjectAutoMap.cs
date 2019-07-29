using AutoMapper;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DeleteBoilerplate;
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
                .ForMember(dst => dst.Year, opt => opt.MapFrom(src => src.Year));

            CreateMap<Project, IMetadata>(MemberList.None)
                .IncludeBase<TreeNode, IMetadata>()
                .ForMember(dst => dst.CanonicalUrl, opt => opt.MapFrom(src => src.SeoUrl.GetAbsoluteAppUrl()));
        }
    }
}