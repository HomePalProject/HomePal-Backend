using HomePal.Domain.Entities.Recipe;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations.Recipe;

public class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.ToTable("RecipeSteps");

        builder.HasKey(rs => rs.Id);

        builder.Property(rs => rs.StepOrder)
            .IsRequired();

        builder.Property(rs => rs.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(rs => new
        {
            rs.RecipeId,
            rs.StepOrder
        }).IsUnique();
    }
}