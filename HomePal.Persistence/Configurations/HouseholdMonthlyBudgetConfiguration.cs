using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class HouseholdMonthlyBudgetConfiguration : IEntityTypeConfiguration<HouseholdMonthlyBudget>
{
    public void Configure(EntityTypeBuilder<HouseholdMonthlyBudget> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BudgetDate)
            .IsRequired();

        builder.Property(b => b.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.Notes)
            .HasMaxLength(500);

        builder.HasIndex(b => new { b.HouseholdId, b.BudgetDate })
            .IsUnique();

        builder.HasOne(b => b.Household)
            .WithMany(h => h.Budgets)
            .HasForeignKey(b => b.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
