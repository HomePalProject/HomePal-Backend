using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class PantryRepository : Repository<Pantry>, IPantryRepository
{
    public PantryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Pantry?> GetByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.HouseholdId == householdId, cancellationToken);
    }

    public async Task<Pantry?> GetByMemberUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Pantries
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Household.Members.Any(m => m.UserId == userId), cancellationToken);
    }
}
