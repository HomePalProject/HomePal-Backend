using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using HomePal.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class OfferRepository : Repository<Offer>, IOfferRepository
{
    public OfferRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Offer?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Supermarket)
            .Include(o => o.Category)
            .Include(o => o.Unit)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<PaginatedList<Offer>> GetPagedAsync(
        PaginationRequest paginationRequest,
        string? query = null,
        Guid? categoryId = null,
        Guid? supermarketId = null,
        bool onlyVerified = true,
        CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking()
            .Include(o => o.Supermarket)
            .Include(o => o.Category)
            .Include(o => o.Unit)
            .AsQueryable();

        if (onlyVerified)
        {
            dbQuery = dbQuery.Where(o => o.IsVerified);
        }

        if (categoryId.HasValue)
        {
            dbQuery = dbQuery.Where(o => o.CategoryId == categoryId.Value);
        }

        if (supermarketId.HasValue)
        {
            dbQuery = dbQuery.Where(o => o.SupermarketId == supermarketId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            dbQuery = dbQuery.Where(o => o.Name.Any(x => x.Value.Contains(term)) ||
                                        (o.Description != null && o.Description.Any(x => x.Value.Contains(term))));
        }

        var count = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .OrderByDescending(o => o.CreatedAt)
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<Offer>.Create(items, count, paginationRequest.PageNumber, paginationRequest.PageSize);
    }

    public async Task<List<Offer>> SearchSemanticAsync(
        Microsoft.Data.SqlTypes.SqlVector<float>? queryEmbedding,
        int take = 10,
        CancellationToken cancellationToken = default)
    {
        if (queryEmbedding == null)
        {
            return new List<Offer>();
        }

        take = Math.Clamp(take, 1, 50);
        var nonNullEmbedding = queryEmbedding.Value;

        return await _dbSet.AsNoTracking()
            .Include(o => o.Supermarket)
            .Include(o => o.Category)
            .Include(o => o.Unit)
            .Where(o => o.IsVerified && o.Embedding != null)
            .OrderBy(o => EF.Functions.VectorDistance<float>("cosine", o.Embedding!.Value, nonNullEmbedding))
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
