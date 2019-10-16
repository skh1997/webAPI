using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Core.Bases;

namespace Sales.Infrastructure.EntityConfigurations
{
    public abstract class EntityBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : EntityBase
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            ConfigureDerived(builder);
        }

        public abstract void ConfigureDerived(EntityTypeBuilder<T> b);
    }
}