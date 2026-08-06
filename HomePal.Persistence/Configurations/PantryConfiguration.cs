using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class PantryConfiguration : IEntityTypeConfiguration<Pantry>
{
    public void Configure(EntityTypeBuilder<Pantry> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.HouseholdId)
            .IsUnique();

        builder.HasOne(p => p.Household)
            .WithOne(h => h.Pantry)
            .HasForeignKey<Pantry>(p => p.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Items)
            .WithOne(i => i.Pantry)
            .HasForeignKey(i => i.PantryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
