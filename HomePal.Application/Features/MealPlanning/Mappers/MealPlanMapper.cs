using HomePal.Application.Features.MealPlanning.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.MealPlanning.Mappers;

public static class MealPlanMapper
{
    public static MealPlanResponse ToResponse(this MealPlan entity)
    {
        return new MealPlanResponse
        {
            Id = entity.Id,
            HouseholdId = entity.HouseholdId,
            Title = entity.Title,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            TotalEstimatedCost = entity.TotalEstimatedCost,
            PlanData = entity.PlanData,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static MealPlan ToEntity(this CreateMealPlanRequest request, Guid householdId)
    {
        return new MealPlan
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            Title = request.Title.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalEstimatedCost = request.TotalEstimatedCost,
            PlanData = request.PlanData,
            CreatedAt = DateTime.UtcNow
        };
    }
}
