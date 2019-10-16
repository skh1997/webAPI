using AutoMapper;
using Sales.Api.ViewModels;
using Sales.Core.DomainModels;

namespace Sales.Api.Configurations
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName => "DomainToViewModelMappings";

        public DomainToViewModelMappingProfile()
        {
            CreateMap<Product, ProductViewModel>();
            CreateMap<Vehicle, VehicleViewModel>();
            CreateMap<Customer, CustomerViewModel>();
        }
    }
}