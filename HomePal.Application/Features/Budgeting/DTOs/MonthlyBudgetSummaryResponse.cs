namespace HomePal.Application.Features.Budgeting.DTOs;

public class MonthlyBudgetSummaryResponse
{
    public Guid? BudgetId { get; set; }
    public Guid HouseholdId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal RemainingAmount => BudgetAmount - TotalSpent;
    public string? Notes { get; set; }
    public int TotalExpensesCount { get; set; }
    public List<ExpenseResponse> RecentExpenses { get; set; } = new();
}
