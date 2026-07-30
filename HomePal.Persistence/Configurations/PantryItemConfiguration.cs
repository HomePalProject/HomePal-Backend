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
            .HasMaxLength(150);

        builder.Property(pi => pi.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(pi => pi.MeasuringUnit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pi => pi.Category)
            .IsRequired()
            .HasMaxLength(100);
    }
}
