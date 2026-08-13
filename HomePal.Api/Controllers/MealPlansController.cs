using HomePal.Application.Features.MealPlanning.DTOs;
using HomePal.Application.Features.MealPlanning.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Pagination;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/meal-plans")]
public class MealPlansController : BaseApiController
{
    private readonly IMealPlanService _mealPlanService;

    public MealPlansController(IMealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    /// <summary>
    /// Create a new meal plan for the household (Household Manager only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<MealPlanResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMealPlan([FromBody] CreateMealPlanRequest request, CancellationToken cancellationToken)
    {
        var result = await _mealPlanService.CreateMealPlanAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all meal plans for the household (paginated)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<MealPlanResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMealPlans([FromQuery] PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        var result = await _mealPlanService.GetMealPlansAsync(CurrentUserId, paginationRequest, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get the last (latest created) meal plan for the household
    /// </summary>
    [HttpGet("last")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<MealPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLastMealPlan(CancellationToken cancellationToken)
    {
        var result = await _mealPlanService.GetLastMealPlanAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific meal plan by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<MealPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMealPlanById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mealPlanService.GetMealPlanByIdAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing meal plan (Household Manager only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<MealPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMealPlan(Guid id, [FromBody] UpdateMealPlanRequest request, CancellationToken cancellationToken)
    {
        var result = await _mealPlanService.UpdateMealPlanAsync(CurrentUserId, id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a meal plan by ID (Household Manager only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMealPlan(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mealPlanService.DeleteMealPlanAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }
}
