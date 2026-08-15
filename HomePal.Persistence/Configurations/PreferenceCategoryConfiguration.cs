using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class PreferenceCategoryConfiguration : IEntityTypeConfiguration<PreferenceCategory>
{
    public void Configure(EntityTypeBuilder<PreferenceCategory> builder)
    {
        builder.HasKey(c => c.Id);

        builder.OwnsMany(c => c.Name, n => n.ToJson());
        builder.OwnsMany(c => c.Description, d => d.ToJson());

        builder.HasMany(c => c.Preferences)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


