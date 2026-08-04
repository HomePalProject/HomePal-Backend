using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using HomePal.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class CanonicalProductRepository : Repository<CanonicalProduct>, ICanonicalProductRepository
{
    public CanonicalProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<CanonicalProduct?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Offers)
                .ThenInclude(o => o.Supermarket)
            .Include(p => p.Offers)
                .ThenInclude(o => o.Unit)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PaginatedList<CanonicalProduct>> GetPagedAsync(
        PaginationRequest paginationRequest,
        string? query = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Offers)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            dbQuery = dbQuery.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            dbQuery = dbQuery.Where(p => p.Name.Any(x => x.Value.Contains(term)) ||
                                        (p.Description != null && p.Description.Any(x => x.Value.Contains(term))));
        }

        var count = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<CanonicalProduct>.Create(items, count, paginationRequest.PageNumber, paginationRequest.PageSize);
    }
}
