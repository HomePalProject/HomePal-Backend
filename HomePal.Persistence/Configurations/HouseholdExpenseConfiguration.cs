using HomePal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomePal.Persistence.Configurations;

public class HouseholdExpenseConfiguration : IEntityTypeConfiguration<HouseholdExpense>
{
    public void Configure(EntityTypeBuilder<HouseholdExpense> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(e => e.Household)
            .WithMany(h => h.Expenses)
            .HasForeignKey(e => e.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Budget)
            .WithMany(b => b.Expenses)
            .HasForeignKey(e => e.BudgetId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
