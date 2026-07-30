using HomePal.Domain.Entities.Recipe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Difficulty)
            .IsRequired();

        builder.Property(r => r.TimeToMake)
            .IsRequired();

        builder.Property(r => r.Servings)
            .IsRequired();

        builder.Property(r => r.ImageUrl)
            .HasMaxLength(500);

        builder.HasMany(r => r.Ingredients)
            .WithOne(ri => ri.Recipe)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Steps)
            .WithOne(rs => rs.Recipe)
            .HasForeignKey(rs => rs.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}