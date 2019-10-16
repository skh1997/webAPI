using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Core.DomainModels;
using Sales.Infrastructure.Data;

namespace Sales.Infrastructure.EntityConfigurations
{
    public class CustomerConfiguration : EntityBaseConfiguration<Customer>
    {
        public override void ConfigureDerived(EntityTypeBuilder<Customer> b)
        {
            b.Property(x => x.Company).IsRequired().HasMaxLength(100);
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        }
    }
}