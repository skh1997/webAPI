using System;
using System.Collections.Generic;
using Sales.Core.DomainModels;
using Sales.Infrastructure.UsefulModels.Pagination;

namespace Sales.Api.ViewModels.PropertyMappings
{
    public class ProductPropertyMapping : PropertyMapping
    {
        public ProductPropertyMapping() : base(
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(ProductViewModel.Name), new PropertyMappingValue(new [] { nameof(Product.Name) } )},
                    { nameof(ProductViewModel.EquivalentTon), new PropertyMappingValue(new [] { nameof(Product.EquivalentTon) } )},
                    { nameof(ProductViewModel.ShelfLife), new PropertyMappingValue(new [] { nameof(Product.ShelfLife) } )},
                    { nameof(ProductViewModel.TaxRate), new PropertyMappingValue(new [] { nameof(Product.TaxRate) } )},
                })
        {
        }
    }
}
