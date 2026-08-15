using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class HouseholdRepository : Repository<Household>, IHouseholdRepository
{
    public HouseholdRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Household?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(h => h.Governorate)
            .Include(h => h.City)
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<Household?> GetByMemberUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(h => h.Governorate)
            .Include(h => h.City)
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Members.Any(m => m.UserId == userId), cancellationToken);
    }
}
