namespace HomePal.Application.Features.Budgeting.DTOs;

public class ExpenseResponse
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid? BudgetId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
