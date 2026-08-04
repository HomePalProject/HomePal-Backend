using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class ProductCategoryRepository : Repository<ProductCategory>, IProductCategoryRepository
{
    public ProductCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductCategory>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            dbQuery = dbQuery.Where(c => c.Name.Any(x => x.Value.Contains(term)) ||
                                        (c.Description != null && c.Description.Any(x => x.Value.Contains(term))));
        }

        return await dbQuery.ToListAsync(cancellationToken);
    }
}
