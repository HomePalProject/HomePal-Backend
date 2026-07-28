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
        return await _dbSet.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Preference?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower().Trim(), cancellationToken);
    }

    public override async Task<IReadOnlyList<Preference>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Preference>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Preference>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllAsync(cancellationToken);
        }

        var term = query.Trim().ToLower();
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Name.ToLower().Contains(term) || (p.Description != null && p.Description.ToLower().Contains(term)))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
