using AuthorizationServer.Models;
using AuthorizationServer.ViewModels;
using AutoMapper;

namespace AuthorizationServer.Configuration
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName => "ViewModelToDomainMappings";

        public ViewModelToDomainMappingProfile()
        {
            CreateMap<UserViewModel, ApplicationUser>();
        }
    }
}