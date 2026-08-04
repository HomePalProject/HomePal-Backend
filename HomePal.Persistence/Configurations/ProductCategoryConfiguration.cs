using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(c => c.Id);

        builder.OwnsMany(c => c.Name, n => n.ToJson());
        builder.OwnsMany(c => c.Description, d => d.ToJson());

        builder.Property(c => c.ImagePath).HasMaxLength(500);
    }
}
