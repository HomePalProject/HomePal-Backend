namespace HomePal.Domain.Entities;

public class HouseholdMonthlyBudget
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;

    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<HouseholdExpense> Expenses { get; set; } = new List<HouseholdExpense>();
}
