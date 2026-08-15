using HomePal.Application.Features.Locations.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class GovernorateRepository : Repository<Governorate>, IGovernorateRepository
{
    public GovernorateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Governorate>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking().Include(g => g.Cities).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            dbQuery = dbQuery.Where(g =>
                g.Code.Contains(term) ||
                g.Name.Any(n => n.Value.Contains(term)));
        }

        return await dbQuery.ToListAsync(cancellationToken);
    }

    public async Task<Governorate?> GetByIdWithCitiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Cities)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<Governorate?> GetByCodeAsync(string code, bool includeCities = false, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking().AsQueryable();

        if (includeCities)
        {
            dbQuery = dbQuery.Include(g => g.Cities);
        }

        return await dbQuery.FirstOrDefaultAsync(g => g.Code.ToLower() == code.Trim().ToLower(), cancellationToken);
    }
}
