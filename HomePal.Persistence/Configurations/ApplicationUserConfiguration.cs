using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Gender)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.HasOne(u => u.Governorate)
            .WithMany()
            .HasForeignKey(u => u.GovernorateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.City)
            .WithMany()
            .HasForeignKey(u => u.CityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
