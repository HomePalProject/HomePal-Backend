using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(i => i.Id);

        builder.OwnsMany(i => i.Name, n => n.ToJson());
        builder.OwnsMany(i => i.Description, d => d.ToJson());

        builder.Property(i => i.PictureUrl)
            .HasMaxLength(500);
    }
}
