using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.MealPlanning.Interfaces;

public interface IMealPlanRepository : IRepository<MealPlan>
{
    Task<MealPlan?> GetByIdAndHouseholdIdAsync(Guid id, Guid householdId, CancellationToken cancellationToken = default);
    Task<PaginatedList<MealPlan>> GetPagedByHouseholdIdAsync(Guid householdId, PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
    Task<MealPlan?> GetLastByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default);
}
