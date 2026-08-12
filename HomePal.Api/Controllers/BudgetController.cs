using HomePal.Application.Features.Budgeting.DTOs;
using HomePal.Application.Features.Budgeting.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/budget")]
public class BudgetController : BaseApiController
{
    private readonly IBudgetService _budgetService;

    public BudgetController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    /// <summary>
    /// Get household monthly budget summary including target, total spent, remaining balance, and expenses
    /// </summary>
    [HttpGet("summary")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<MonthlyBudgetSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonthlyBudgetSummary([FromQuery] int? year, [FromQuery] int? month, CancellationToken cancellationToken)
    {
        var result = await _budgetService.GetMonthlyBudgetSummaryAsync(CurrentUserId, year, month, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Set or update the household target budget amount for a specific month and year
    /// </summary>
    [HttpPost("target")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<MonthlyBudgetSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetMonthlyBudget([FromBody] SetMonthlyBudgetRequest request, CancellationToken cancellationToken)
    {
        var result = await _budgetService.SetMonthlyBudgetAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all logged expenses for a specific month and year
    /// </summary>
    [HttpGet("expenses")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<List<ExpenseResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenses([FromQuery] int? year, [FromQuery] int? month, CancellationToken cancellationToken)
    {
        var result = await _budgetService.GetExpensesAsync(CurrentUserId, year, month, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Log a manual expense for the household
    /// </summary>
    [HttpPost("expenses")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddExpense([FromBody] CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        var result = await _budgetService.AddExpenseAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing manual expense
    /// </summary>
    [HttpPut("expenses/{id}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request, CancellationToken cancellationToken)
    {
        var result = await _budgetService.UpdateExpenseAsync(CurrentUserId, id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete an expense
    /// </summary>
    [HttpDelete("expenses/{id}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExpense(Guid id, CancellationToken cancellationToken)
    {
        var result = await _budgetService.DeleteExpenseAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }
}
