using AutoMapper;
using CMS.ContactManagement;
using DeleteBoilerplate.Account.Infrastructure;
using DeleteBoilerplate.Account.Models;

namespace DeleteBoilerplate.Account
{
    public class AccountAutoMap : AutoMapper.Profile
    {
        public AccountAutoMap()
        {
            this.CreateMapAccount();
        }

        private void CreateMapAccount()
        {
            CreateMap<AppUser, ContactInfo>(MemberList.None)
                .ForMember(dst => dst.ContactEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.ContactFirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dst => dst.ContactLastName, opt => opt.MapFrom(src => src.LastName))
                ;

            CreateMap<RegisterViewModel, AppUser>(MemberList.None)
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.Enabled, opt => opt.MapFrom(src => true))
                ;

            CreateMap<AppUser, UserViewModel>(MemberList.None)
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                ;

            CreateMap<AppUser, UpdateUserDetailsViewModel>(MemberList.None)
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                ;

            CreateMap<UpdateUserDetailsViewModel, AppUser>(MemberList.None)
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.LastName))
                // .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                ;
        }
    }
}