using AutoMapper;
using DeleteBoilerplate.AzureSearch.Models.CompanyMember;
using DeleteBoilerplate.CompanyMembers.Models;

namespace DeleteBoilerplate.CompanyMembers
{
    public class CompanyMembersAutoMap : AutoMapper.Profile
    {
        public CompanyMembersAutoMap()
        {
            this.CreateMapCompanyMembers();
        }

        private void CreateMapCompanyMembers()
        {
            CreateMap<CompanyMemberInfo, CompanyMemberViewModel>(MemberList.None)
                .ForMember(dst => dst.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dst => dst.PersonalIdentifier, opt => opt.MapFrom(src => src.PersonalIdentifier))
                .ForMember(dst => dst.Team, opt => opt.MapFrom(src => src.Team));

            CreateMap<CompanyMemberAzureSearchResultItem, CompanyMemberViewModel>(MemberList.None)
                .ForMember(dst => dst.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dst => dst.PersonalIdentifier, opt => opt.MapFrom(src => src.PersonalIdentifier))
                .ForMember(dst => dst.Team, opt => opt.MapFrom(src => src.Team));
        }
    }
}