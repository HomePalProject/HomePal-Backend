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

        builder.HasOne(h => h.Governorate)
            .WithMany()
            .HasForeignKey(h => h.GovernorateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(h => h.City)
            .WithMany()
            .HasForeignKey(h => h.CityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(h => h.Members)
            .WithOne(m => m.Household)
            .HasForeignKey(m => m.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Invitations)
            .WithOne(i => i.Household)
            .HasForeignKey(i => i.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
