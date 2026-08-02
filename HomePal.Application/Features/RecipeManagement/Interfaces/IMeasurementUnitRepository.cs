using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.RecipeManagement.Interfaces;

public interface IMeasurementUnitRepository : IRepository<MeasurementUnit>
{
    Task<PaginatedList<MeasurementUnit>> GetPaginatedAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default);
    Task<bool> IsUsedInRecipesAsync(Guid unitId, CancellationToken cancellationToken = default);
}
