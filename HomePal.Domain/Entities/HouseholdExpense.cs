namespace HomePal.Domain.Entities;

public class HouseholdExpense
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;

    public Guid? BudgetId { get; set; }
    public HouseholdMonthlyBudget? Budget { get; set; }

    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
