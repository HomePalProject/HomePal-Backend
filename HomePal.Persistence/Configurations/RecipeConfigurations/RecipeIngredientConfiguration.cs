using HomePal.Domain.Entities.Recipe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredients");

        builder.HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        builder.Property(ri => ri.Amount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(ri => ri.Unit)
            .IsRequired();
    }
}