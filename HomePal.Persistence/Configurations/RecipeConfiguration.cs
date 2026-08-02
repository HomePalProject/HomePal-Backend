using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.OwnsMany(r => r.Name, n => n.ToJson());
        builder.OwnsMany(r => r.Description, d => d.ToJson());
        builder.OwnsMany(r => r.Steps, s => s.ToJson());

        builder.Property(r => r.ImageUrl)
            .HasMaxLength(500);

        builder.HasMany(r => r.Preferences)
            .WithMany(p => p.Recipes)
            .UsingEntity(j => j.ToTable("RecipePreferences"));
    }
}
