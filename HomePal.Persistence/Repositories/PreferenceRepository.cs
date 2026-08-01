using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class PreferenceRepository : Repository<Preference>, IPreferenceRepository
{
    public PreferenceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Preference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Preference?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var trimmed = name.Trim();
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Name.Any(x => x.Value.Contains(trimmed)), cancellationToken);
    }

    public override async Task<IReadOnlyList<Preference>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Preference>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Preference>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Preference>> SearchAsync(string query, Guid? categoryId = null, CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking().Include(p => p.Category).AsQueryable();

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

        return await dbQuery.ToListAsync(cancellationToken);
    }
}





