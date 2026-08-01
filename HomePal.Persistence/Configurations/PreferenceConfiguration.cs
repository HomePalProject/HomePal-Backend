using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.HasKey(p => p.Id);

        builder.OwnsMany(p => p.Name, n => n.ToJson());
        builder.OwnsMany(p => p.Description, d => d.ToJson());

        builder.HasMany(p => p.Members)
            .WithMany(m => m.Preferences)
            .UsingEntity(j => j.ToTable("HouseholdMemberPreferences"));
    }
}


