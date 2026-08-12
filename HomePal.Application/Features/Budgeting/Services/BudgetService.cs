using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Budgeting.DTOs;
using HomePal.Application.Features.Budgeting.Interfaces;
using HomePal.Application.Features.Budgeting.Mappers;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Budgeting.Services;

public class BudgetService : IBudgetService
{
    private readonly IUnitOfWork _unitOfWork;

    public BudgetService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MonthlyBudgetSummaryResponse>> GetMonthlyBudgetSummaryAsync(
        Guid userId, 
        int? year = null, 
        int? month = null, 
        CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<MonthlyBudgetSummaryResponse>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var now = DateTime.UtcNow;
        int targetYear = year ?? now.Year;
        int targetMonth = month ?? now.Month;

        var budget = await _unitOfWork.MonthlyBudgets.GetByHouseholdAndPeriodAsync(householdId.Value, targetYear, targetMonth, cancellationToken);

        decimal budgetAmount = budget?.Amount ?? 0m;

        if (budget == null)
        {
            var previousBudget = await _unitOfWork.MonthlyBudgets.GetLatestBeforePeriodAsync(householdId.Value, targetYear, targetMonth, cancellationToken);
            if (previousBudget != null)
            {
                budgetAmount = previousBudget.Amount;
            }
        }

        var expenses = await _unitOfWork.HouseholdExpenses.GetByHouseholdAndPeriodAsync(householdId.Value, targetYear, targetMonth, cancellationToken);
        decimal totalSpent = expenses.Sum(e => e.Amount);

        var summary = new MonthlyBudgetSummaryResponse
        {
            BudgetId = budget?.Id,
            HouseholdId = householdId.Value,
            Year = targetYear,
            Month = targetMonth,
            BudgetAmount = budgetAmount,
            TotalSpent = totalSpent,
            Notes = budget?.Notes,
            TotalExpensesCount = expenses.Count,
            RecentExpenses = expenses.Select(e => e.ToResponse()).ToList()
        };

        return Result<MonthlyBudgetSummaryResponse>.Ok(summary, SuccessMessages.General);
    }

    public async Task<Result<MonthlyBudgetSummaryResponse>> SetMonthlyBudgetAsync(
        Guid userId, 
        SetMonthlyBudgetRequest request, 
        CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<MonthlyBudgetSummaryResponse>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        if (request.Year < 2000 || request.Year > 2100 || request.Month < 1 || request.Month > 12)
        {
            return Result<MonthlyBudgetSummaryResponse>.Fail(ErrorMessages.Budget.InvalidYearOrMonth, ResultStatus.BadRequest);
        }

        if (request.TargetAmount < 0)
        {
            return Result<MonthlyBudgetSummaryResponse>.Fail(ErrorMessages.Budget.InvalidAmount, ResultStatus.BadRequest);
        }

        var existingBudget = await _unitOfWork.MonthlyBudgets.GetByHouseholdAndPeriodAsync(householdId.Value, request.Year, request.Month, cancellationToken);
        if (existingBudget != null)
        {
            existingBudget.UpdateEntity(request);
            _unitOfWork.MonthlyBudgets.Update(existingBudget);
        }
        else
        {
            var newBudget = request.ToEntity(householdId.Value);
            await _unitOfWork.MonthlyBudgets.AddAsync(newBudget, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetMonthlyBudgetSummaryAsync(userId, request.Year, request.Month, cancellationToken);
    }

    public async Task<Result<ExpenseResponse>> AddExpenseAsync(
        Guid userId, 
        CreateExpenseRequest request, 
        CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<ExpenseResponse>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<ExpenseResponse>.Fail(ErrorMessages.Budget.InvalidTitle, ResultStatus.BadRequest);
        }

        if (request.Amount <= 0)
        {
            return Result<ExpenseResponse>.Fail(ErrorMessages.Budget.InvalidAmount, ResultStatus.BadRequest);
        }

        var expenseDate = request.ExpenseDate ?? DateTime.UtcNow;
        var budget = await _unitOfWork.MonthlyBudgets.GetByHouseholdAndPeriodAsync(householdId.Value, expenseDate.Year, expenseDate.Month, cancellationToken);

        var newExpense = request.ToEntity(householdId.Value, budget?.Id);

        await _unitOfWork.HouseholdExpenses.AddAsync(newExpense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var saved = await _unitOfWork.HouseholdExpenses.GetByIdAsync(newExpense.Id, cancellationToken) ?? newExpense;
        return Result<ExpenseResponse>.Ok(saved.ToResponse(), SuccessMessages.General, ResultStatus.Created);
    }

    public async Task<Result<ExpenseResponse>> UpdateExpenseAsync(
        Guid userId, 
        Guid expenseId, 
        UpdateExpenseRequest request, 
        CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<ExpenseResponse>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var expense = await _unitOfWork.HouseholdExpenses.GetByIdAsync(expenseId, cancellationToken);
        if (expense == null || expense.HouseholdId != householdId.Value)
        {
            return Result<ExpenseResponse>.Fail(ErrorMessages.Budget.ExpenseNotFound, ResultStatus.NotFound);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<ExpenseResponse>.Fail(ErrorMessages.Budget.InvalidTitle, ResultStatus.BadRequest);
        }

        if (request.Amount <= 0)
        {
            return Result<ExpenseResponse>.Fail(ErrorMessages.Budget.InvalidAmount, ResultStatus.BadRequest);
        }

        var expenseDate = request.ExpenseDate ?? expense.ExpenseDate;
        var budget = await _unitOfWork.MonthlyBudgets.GetByHouseholdAndPeriodAsync(householdId.Value, expenseDate.Year, expenseDate.Month, cancellationToken);

        expense.UpdateEntity(request, budget?.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ExpenseResponse>.Ok(expense.ToResponse(), SuccessMessages.General);
    }

    public async Task<Result> DeleteExpenseAsync(Guid userId, Guid expenseId, CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var expense = await _unitOfWork.HouseholdExpenses.GetByIdAsync(expenseId, cancellationToken);
        if (expense == null || expense.HouseholdId != householdId.Value)
        {
            return Result.Fail(ErrorMessages.Budget.ExpenseNotFound, ResultStatus.NotFound);
        }

        _unitOfWork.HouseholdExpenses.Remove(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.General);
    }

    public async Task<Result<List<ExpenseResponse>>> GetExpensesAsync(
        Guid userId, 
        int? year = null, 
        int? month = null, 
        CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<List<ExpenseResponse>>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var now = DateTime.UtcNow;
        int targetYear = year ?? now.Year;
        int targetMonth = month ?? now.Month;

        var expenses = await _unitOfWork.HouseholdExpenses.GetByHouseholdAndPeriodAsync(householdId.Value, targetYear, targetMonth, cancellationToken);
        var dtos = expenses.Select(e => e.ToResponse()).ToList();

        return Result<List<ExpenseResponse>>.Ok(dtos, SuccessMessages.General);
    }

    private async Task<Guid?> GetUserHouseholdIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        return member?.HouseholdId;
    }
}
