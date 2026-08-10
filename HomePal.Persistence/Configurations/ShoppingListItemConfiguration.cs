using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class ShoppingListItemConfiguration : IEntityTypeConfiguration<ShoppingListItem>
{
    public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(s => s.Quantity)
            .IsRequired();

        builder.Property(s => s.PortionCount)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(s => s.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.HasOne(s => s.ShoppingList)
            .WithMany(l => l.Items)
            .HasForeignKey(s => s.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.MeasuringUnit)
            .WithMany()
            .HasForeignKey(s => s.MeasuringUnitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Offer)
            .WithMany()
            .HasForeignKey(s => s.OfferId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
