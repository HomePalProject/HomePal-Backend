using HomePal.Application.Features.MealPlanning.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using HomePal.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class MealPlanRepository : Repository<MealPlan>, IMealPlanRepository
{
    public MealPlanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MealPlan?> GetByIdAndHouseholdIdAsync(Guid id, Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.HouseholdId == householdId, cancellationToken);
    }

    public async Task<PaginatedList<MealPlan>> GetPagedByHouseholdIdAsync(Guid householdId, PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet
            .AsNoTracking()
            .Where(m => m.HouseholdId == householdId);

        var count = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .OrderByDescending(m => m.CreatedAt)
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<MealPlan>.Create(items, count, paginationRequest.PageNumber, paginationRequest.PageSize);
    }

    public async Task<MealPlan?> GetLastByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.HouseholdId == householdId)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
