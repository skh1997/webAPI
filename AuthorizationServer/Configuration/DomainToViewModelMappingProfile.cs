using AuthorizationServer.Models;
using AuthorizationServer.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Configuration
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName => "DomainToViewModelMappings";

        public DomainToViewModelMappingProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>();
        }
    }
}