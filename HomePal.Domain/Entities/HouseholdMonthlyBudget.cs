using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class HouseholdMonthlyBudget : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;

    public DateTime BudgetDate { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }

    public ICollection<HouseholdExpense> Expenses { get; set; } = new List<HouseholdExpense>();
}
