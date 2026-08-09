using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.HasKey(o => o.Id);

        builder.OwnsMany(o => o.Name, n => n.ToJson());
        builder.OwnsMany(o => o.Description, d => d.ToJson());

        builder.Property(o => o.OriginalPrice)
            .HasPrecision(18, 2);

        builder.Property(o => o.DiscountedPrice)
            .HasPrecision(18, 2);

        builder.Property(o => o.Embedding)
            .HasColumnType("vector(1536)");

        builder.Property(o => o.IsVerified)
            .HasDefaultValue(true);

        builder.HasOne(o => o.Supermarket)
            .WithMany(s => s.Offers)
            .HasForeignKey(o => o.SupermarketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Category)
            .WithMany(c => c.Offers)
            .HasForeignKey(o => o.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.Unit)
            .WithMany(u => u.Offers)
            .HasForeignKey(o => o.UnitId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
