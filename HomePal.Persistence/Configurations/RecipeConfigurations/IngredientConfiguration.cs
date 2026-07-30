using HomePal.Domain.Entities.Recipe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations.Recipe;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredients");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.DefaultUnit)
            .IsRequired();

        builder.Property(i => i.Category)
            .IsRequired();

        builder.HasIndex(i => i.Name)
            .IsUnique();

        builder.HasMany(i => i.Recipes)
            .WithOne(ri => ri.Ingredient)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}