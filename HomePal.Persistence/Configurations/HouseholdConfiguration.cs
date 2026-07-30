using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class HouseholdConfiguration : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(h => h.Address)
            .HasMaxLength(250);

        builder.Property(h => h.Governorate)
            .HasMaxLength(100);

        builder.Property(h => h.City)
            .HasMaxLength(100);

        builder.HasMany(h => h.Members)
            .WithOne(m => m.Household)
            .HasForeignKey(m => m.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Invitations)
            .WithOne(i => i.Household)
            .HasForeignKey(i => i.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.Pantry)
            .WithOne(p => p.Household)
            .HasForeignKey<Pantry>(p => p.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
