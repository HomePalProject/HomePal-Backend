using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using HomePal.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class MeasurementUnitRepository : Repository<MeasurementUnit>, IMeasurementUnitRepository
{
    public MeasurementUnitRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PaginatedList<MeasurementUnit>> GetPaginatedAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(m => m.Name.Any(n => n.Value.ToLower().Contains(searchLower)) ||
                                     (m.Symbol != null && m.Symbol.Any(s => s.Value.ToLower().Contains(searchLower))));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(m => m.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<MeasurementUnit>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<bool> IsUsedInRecipesAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        return await _context.RecipeIngredients.AnyAsync(ri => ri.MeasurementUnitId == unitId, cancellationToken);
    }
}
