namespace HomePal.Application.Features.Budgeting.DTOs;

public class SetMonthlyBudgetRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TargetAmount { get; set; }
    public string? Notes { get; set; }
}
