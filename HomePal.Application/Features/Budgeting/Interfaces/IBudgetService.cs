using HomePal.Application.Features.Budgeting.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Budgeting.Interfaces;

public interface IBudgetService
{
    Task<Result<MonthlyBudgetSummaryResponse>> GetMonthlyBudgetSummaryAsync(Guid userId, int? year = null, int? month = null, CancellationToken cancellationToken = default);
    Task<Result<MonthlyBudgetSummaryResponse>> SetMonthlyBudgetAsync(Guid userId, SetMonthlyBudgetRequest request, CancellationToken cancellationToken = default);
    Task<Result<ExpenseResponse>> AddExpenseAsync(Guid userId, CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<Result<ExpenseResponse>> UpdateExpenseAsync(Guid userId, Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteExpenseAsync(Guid userId, Guid expenseId, CancellationToken cancellationToken = default);
    Task<Result<List<ExpenseResponse>>> GetExpensesAsync(Guid userId, int? year = null, int? month = null, CancellationToken cancellationToken = default);
}
