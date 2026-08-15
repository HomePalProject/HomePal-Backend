using HomePal.Application.Features.Locations.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<City>> SearchAsync(Guid? governorateId = null, string? governorateCode = null, string? query = null, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking().Include(c => c.Governorate).AsQueryable();

        if (governorateId.HasValue && governorateId.Value != Guid.Empty)
        {
            dbQuery = dbQuery.Where(c => c.GovernorateId == governorateId.Value);
        }

        if (!string.IsNullOrWhiteSpace(governorateCode))
        {
            var code = governorateCode.Trim().ToLower();
            dbQuery = dbQuery.Where(c => c.GovernorateCode.ToLower() == code);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            dbQuery = dbQuery.Where(c => c.Name.Any(n => n.Value.Contains(term)));
        }

        return await dbQuery.ToListAsync(cancellationToken);
    }

    public async Task<City?> GetByIdWithGovernorateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.Governorate)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
