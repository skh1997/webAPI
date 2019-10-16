using AutoMapper;
using Sales.Api.ViewModels;
using Sales.Core.DomainModels;

namespace Sales.Api.Configurations
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName => "ViewModelToDomainMappings";

        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ProductViewModel, Product>();
            CreateMap<ProductCreationViewModel, Product>();
            CreateMap<ProductModificationViewModel, Product>();

            CreateMap<VehicleViewModel, Vehicle>();
            CreateMap<CustomerViewModel, Customer>();
        }
    }
}