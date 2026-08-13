using System.ComponentModel;
using HomePal.Application.Features.MealPlanning.DTOs;
using HomePal.Application.Features.MealPlanning.Interfaces;
using HomePal.Infrastructure.AI.Common;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

/// <summary>
/// AI Agent Tool for managing meal plans in the database via IMealPlanService.
/// </summary>
public class MealPlanTools
{
    private readonly IMealPlanService _mealPlanService;
    private readonly AgentUserContext _userContext;

    public MealPlanTools(
        IMealPlanService mealPlanService,
        AgentUserContext userContext)
    {
        _mealPlanService = mealPlanService;
        _userContext = userContext;
    }

    [Description("Saves a new meal plan to the database for the user's household with dates, estimated cost, and plan content.")]
    public async Task<object> SaveMealPlanAsync(
        [Description("Descriptive title for the meal plan (e.g. 'Low-Carb Weekly Plan', 'Family Weekend Dinners').")] string title,
        [Description("Start date of the meal plan (e.g. '2026-08-15').")] DateTime startDate,
        [Description("End date of the meal plan (e.g. '2026-08-21').")] DateTime endDate,
        [Description("The full meal plan content (Markdown formatted text or JSON string).")] string planData,
        [Description("Total estimated cost for all ingredients in the plan.")] decimal totalEstimatedCost = 0,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var request = new CreateMealPlanRequest
        {
            Title = title?.Trim() ?? "Meal Plan",
            StartDate = startDate,
            EndDate = endDate,
            PlanData = planData,
            TotalEstimatedCost = totalEstimatedCost >= 0 ? totalEstimatedCost : 0
        };

        var result = await _mealPlanService.CreateMealPlanAsync(userId, request, cancellationToken);
        if (!result.Success || result.Data == null)
            return new { success = false, error = result.Message };

        return new
        {
            success = true,
            message = "Meal plan saved successfully.",
            mealPlan = result.Data
        };
    }

    [Description("Retrieves the user's most recent saved meal plan from the database.")]
    public async Task<object> GetLastMealPlanAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var result = await _mealPlanService.GetLastMealPlanAsync(userId, cancellationToken);
        if (!result.Success || result.Data == null)
            return new { success = false, error = result.Message ?? "No meal plan found." };

        return new
        {
            success = true,
            mealPlan = result.Data
        };
    }

    [Description("Updates the user's most recent saved meal plan with new title, dates, cost, or plan content.")]
    public async Task<object> UpdateLastMealPlanAsync(
        [Description("Updated title for the meal plan (if changing).")] string? title = null,
        [Description("Updated start date (if changing).")] DateTime? startDate = null,
        [Description("Updated end date (if changing).")] DateTime? endDate = null,
        [Description("Updated full meal plan content (if changing).")] string? planData = null,
        [Description("Updated total estimated cost (if changing).")] decimal? totalEstimatedCost = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var lastPlanResult = await _mealPlanService.GetLastMealPlanAsync(userId, cancellationToken);
        if (!lastPlanResult.Success || lastPlanResult.Data == null)
            return new { success = false, error = "No existing meal plan found to update." };

        var existing = lastPlanResult.Data;
        var request = new UpdateMealPlanRequest
        {
            Title = !string.IsNullOrWhiteSpace(title) ? title.Trim() : existing.Title,
            StartDate = startDate ?? existing.StartDate,
            EndDate = endDate ?? existing.EndDate,
            PlanData = !string.IsNullOrWhiteSpace(planData) ? planData : existing.PlanData,
            TotalEstimatedCost = totalEstimatedCost.HasValue && totalEstimatedCost.Value >= 0 ? totalEstimatedCost.Value : existing.TotalEstimatedCost
        };

        var result = await _mealPlanService.UpdateMealPlanAsync(userId, existing.Id, request, cancellationToken);
        if (!result.Success || result.Data == null)
            return new { success = false, error = result.Message };

        return new
        {
            success = true,
            message = "Meal plan updated successfully.",
            mealPlan = result.Data
        };
    }
}
