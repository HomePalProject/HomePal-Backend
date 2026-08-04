using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class MeasuringUnitRepository : Repository<MeasuringUnit>, IMeasuringUnitRepository
{
    public MeasuringUnitRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<MeasuringUnit>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            dbQuery = dbQuery.Where(u => u.Name.Any(x => x.Value.Contains(term)) ||
                                        u.Symbol.Any(x => x.Value.Contains(term)));
        }

        return await dbQuery.ToListAsync(cancellationToken);
    }
}
