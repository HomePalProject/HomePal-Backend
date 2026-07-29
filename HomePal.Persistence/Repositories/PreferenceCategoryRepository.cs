using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class PreferenceCategoryRepository : Repository<PreferenceCategory>, IPreferenceCategoryRepository
{
    public PreferenceCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<PreferenceCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Preferences)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<PreferenceCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower().Trim(), cancellationToken);
    }

    public async Task<PreferenceCategory?> GetByIdWithPreferencesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Preferences)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<PreferenceCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.Preferences)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PreferenceCategory>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllAsync(cancellationToken);
        }

        var term = query.Trim().ToLower();
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.Preferences)
            .Where(c => c.Name.ToLower().Contains(term) || (c.Description != null && c.Description.ToLower().Contains(term)))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
