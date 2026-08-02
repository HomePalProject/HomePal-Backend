using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        builder.OwnsMany(ri => ri.Notes, n => n.ToJson());

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ri => ri.MeasurementUnit)
            .WithMany(m => m.RecipeIngredients)
            .HasForeignKey(ri => ri.MeasurementUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
