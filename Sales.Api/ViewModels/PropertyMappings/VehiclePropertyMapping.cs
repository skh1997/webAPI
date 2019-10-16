using System;
using System.Collections.Generic;
using Sales.Core.DomainModels;
using Sales.Infrastructure.UsefulModels.Pagination;

namespace Sales.Api.ViewModels.PropertyMappings
{
    public class VehiclePropertyMapping : PropertyMapping
    {
        public VehiclePropertyMapping() : base(
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(VehicleViewModel.Model), new PropertyMappingValue(new [] { nameof(Vehicle.Model) } )},
                    { nameof(VehicleViewModel.Owner), new PropertyMappingValue(new [] { nameof(Vehicle.Owner) } )},
                })
        {
        }
    }
}
