using AutoMapper;
using MindScribe.Models;
using MindScribe.ViewModels;

namespace MindScribe.Extentions
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterViewModel, User>();
        }
    }
}
