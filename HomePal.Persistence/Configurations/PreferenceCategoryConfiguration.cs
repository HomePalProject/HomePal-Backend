using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class PreferenceCategoryConfiguration : IEntityTypeConfiguration<PreferenceCategory>
{
    public void Configure(EntityTypeBuilder<PreferenceCategory> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.Description)
            .HasMaxLength(300);

        builder.HasMany(c => c.Preferences)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
