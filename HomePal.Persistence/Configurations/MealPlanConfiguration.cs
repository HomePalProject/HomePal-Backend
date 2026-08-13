using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.TotalEstimatedCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(m => m.PlanData)
            .IsRequired();

        builder.HasOne(m => m.Household)
            .WithMany(h => h.MealPlans)
            .HasForeignKey(m => m.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
