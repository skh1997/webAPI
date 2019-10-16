using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Core.DomainModels;
using Sales.Infrastructure.Data;

namespace Sales.Infrastructure.EntityConfigurations
{
    public class VehicleConfiguration : EntityBaseConfiguration<Vehicle>
    {
        public override void ConfigureDerived(EntityTypeBuilder<Vehicle> b)
        {
            b.Property(x => x.Model).IsRequired().HasMaxLength(50);
            b.Property(x => x.Owner).IsRequired().HasMaxLength(50);
        }
    }
}