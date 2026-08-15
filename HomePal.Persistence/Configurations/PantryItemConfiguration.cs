using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class PantryItemConfiguration : IEntityTypeConfiguration<PantryItem>
{
    public void Configure(EntityTypeBuilder<PantryItem> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pi => pi.Quantity)
            .HasPrecision(18, 2);

        builder.HasOne(pi => pi.MeasuringUnit)
            .WithMany()
            .HasForeignKey(pi => pi.MeasuringUnitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(pi => pi.Category)
            .WithMany()
            .HasForeignKey(pi => pi.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
