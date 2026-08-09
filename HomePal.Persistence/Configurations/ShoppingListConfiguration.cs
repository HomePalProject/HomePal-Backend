using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingList>
{
    public void Configure(EntityTypeBuilder<ShoppingList> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Household)
            .WithOne(h => h.ShoppingList)
            .HasForeignKey<ShoppingList>(s => s.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
