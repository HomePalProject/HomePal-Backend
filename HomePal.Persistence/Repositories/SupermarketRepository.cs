using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using HomePal.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class SupermarketRepository : Repository<Supermarket>, ISupermarketRepository
{
    public SupermarketRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Supermarket>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            dbQuery = dbQuery.Where(s => s.Name.Any(x => x.Value.Contains(term)) ||
                                        (s.Address != null && s.Address.Contains(term)));
        }

        return await dbQuery.ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<Supermarket>> GetPagedAsync(
        PaginationRequest paginationRequest,
        string? query = null,
        CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            dbQuery = dbQuery.Where(s => s.Name.Any(x => x.Value.Contains(term)) ||
                                        (s.Address != null && s.Address.Contains(term)));
        }

        var count = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .OrderByDescending(s => s.CreatedAt)
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<Supermarket>.Create(items, count, paginationRequest.PageNumber, paginationRequest.PageSize);
    }
}
