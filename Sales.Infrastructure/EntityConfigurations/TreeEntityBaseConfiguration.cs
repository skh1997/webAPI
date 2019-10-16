using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Core.Bases;

namespace Sales.Infrastructure.EntityConfigurations
{
    public abstract class TreeEntityBaseConfiguration<T> : EntityBaseConfiguration<T> where T : TreeEntity<T>
    {
        public override void ConfigureDerived(EntityTypeBuilder<T> builder)
        {

            builder.Property(x => x.AncestorIds).HasMaxLength(200);
            builder.Ignore(x => x.Level);

            builder.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);

            ConfigureTreeDerived(builder);
        }

        protected abstract void ConfigureTreeDerived(EntityTypeBuilder<T> builder);
    }
}
