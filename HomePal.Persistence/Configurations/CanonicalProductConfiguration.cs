using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class CanonicalProductConfiguration : IEntityTypeConfiguration<CanonicalProduct>
{
    public void Configure(EntityTypeBuilder<CanonicalProduct> builder)
    {
        builder.HasKey(p => p.Id);

        builder.OwnsMany(p => p.Name, n => n.ToJson());
        builder.OwnsMany(p => p.Description, d => d.ToJson());

        builder.Property(p => p.Embedding)
            .HasColumnType("vector(1536)");

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.Offers)
            .WithOne(o => o.CanonicalProduct)
            .HasForeignKey(o => o.CanonicalProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
