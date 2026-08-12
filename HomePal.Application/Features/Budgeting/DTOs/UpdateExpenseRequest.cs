namespace HomePal.Application.Features.Budgeting.DTOs;

public class UpdateExpenseRequest
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? ExpenseDate { get; set; }
}
