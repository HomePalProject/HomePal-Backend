using HomePal.Application.Features.Budgeting.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Budgeting.Mappers;

public static class BudgetMapper
{
    public static ExpenseResponse ToResponse(this HouseholdExpense expense)
    {
        return new ExpenseResponse
        {
            Id = expense.Id,
            HouseholdId = expense.HouseholdId,
            BudgetId = expense.BudgetId,
            Title = expense.Title,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            CreatedAt = expense.CreatedAt
        };
    }

    public static HouseholdMonthlyBudget ToEntity(this SetMonthlyBudgetRequest request, Guid householdId)
    {
        return new HouseholdMonthlyBudget
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            BudgetDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc),
            Amount = request.TargetAmount,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static HouseholdExpense ToEntity(this CreateExpenseRequest request, Guid householdId, Guid? budgetId = null)
    {
        return new HouseholdExpense
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            BudgetId = budgetId,
            Title = request.Title.Trim(),
            Amount = request.Amount,
            ExpenseDate = request.ExpenseDate ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this HouseholdMonthlyBudget budget, SetMonthlyBudgetRequest request)
    {
        budget.Amount = request.TargetAmount;
        budget.Notes = request.Notes;
        budget.UpdatedAt = DateTime.UtcNow;
    }

    public static void UpdateEntity(this HouseholdExpense expense, UpdateExpenseRequest request, Guid? budgetId = null)
    {
        expense.Title = request.Title.Trim();
        expense.Amount = request.Amount;
        expense.ExpenseDate = request.ExpenseDate ?? expense.ExpenseDate;
        expense.BudgetId = budgetId;
        expense.UpdatedAt = DateTime.UtcNow;
    }
}
