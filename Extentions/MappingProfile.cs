using AutoMapper;
using MindScribe.Models;
using MindScribe.ViewModels;

namespace MindScribe.Extentions
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login))
                .ForMember(x => x.Email, opt => opt.MapFrom(c => c.EmailReg));
            CreateMap<LoginViewModel, User>();
        }
    }
}
