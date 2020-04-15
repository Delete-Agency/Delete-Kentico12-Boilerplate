using AutoMapper;
using CMS.OnlineForms.Types;
using DeleteBoilerplate.Forms.Models;

namespace DeleteBoilerplate.Forms
{
    public class FormsAutoMap : AutoMapper.Profile
    {
        public FormsAutoMap()
        {
            CreateMap<ContactFormData, ContactItem>(MemberList.None)
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.Telephone, opt => opt.MapFrom(src => src.Telephone))
                .ForMember(dst => dst.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dst => dst.IsConsented, opt => opt.MapFrom(src => src.IsConsented));
        }
    }
}