using System.ComponentModel;
using HomePal.Application.Features.Budgeting.Interfaces;
using HomePal.Infrastructure.AI.Common;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

/// <summary>
/// AI Agent Tool for checking household budget limits via IBudgetService.
/// </summary>
public class BudgetTools
{
    private readonly IBudgetService _budgetService;
    private readonly AgentUserContext _userContext;

    public BudgetTools(
        IBudgetService budgetService,
        AgentUserContext userContext)
    {
        _budgetService = budgetService;
        _userContext = userContext;
    }

    [Description("Retrieves the current month's household grocery budget, total amount spent so far, and remaining available balance. 'hasBudget: false' means no budget has been set for this month — inform the user and skip budget comparisons. 'isOverBudget: true' means total spending exceeds the budget limit — alert the user and suggest cost-saving alternatives.")]
    public async Task<object> GetCurrentBudgetAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var now = DateTime.UtcNow;
        var result = await _budgetService.GetMonthlyBudgetSummaryAsync(userId, now.Year, now.Month, cancellationToken);
        if (!result.Success || result.Data == null)
            return new { success = false, error = result.Message };

        var data = result.Data;
        return new
        {
            success = true,
            year = data.Year,
            month = data.Month,
            hasBudget = data.BudgetId.HasValue,
            monthlyBudget = data.BudgetAmount,
            totalSpent = data.TotalSpent,
            remainingBudget = data.RemainingAmount,
            isOverBudget = data.RemainingAmount < 0,
            totalExpensesCount = data.TotalExpensesCount,
            notes = data.Notes
        };
    }
}
