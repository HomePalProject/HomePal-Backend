using HomePal.Application.Features.MealPlanning.DTOs;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.MealPlanning.Interfaces;

public interface IMealPlanService
{
    Task<Result<MealPlanResponse>> CreateMealPlanAsync(Guid userId, CreateMealPlanRequest request, CancellationToken cancellationToken = default);
    Task<Result<MealPlanResponse>> GetMealPlanByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<MealPlanResponse>>> GetMealPlansAsync(Guid userId, PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
    Task<Result<MealPlanResponse>> GetLastMealPlanAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<MealPlanResponse>> UpdateMealPlanAsync(Guid userId, Guid id, UpdateMealPlanRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteMealPlanAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
