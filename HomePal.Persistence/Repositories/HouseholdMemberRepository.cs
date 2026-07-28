using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class HouseholdMemberRepository : Repository<HouseholdMember>, IHouseholdMemberRepository
{
    public HouseholdMemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<HouseholdMember?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Household)
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);
    }

    public async Task<HouseholdMember?> GetByIdAndHouseholdIdAsync(Guid id, Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id && m.HouseholdId == householdId, cancellationToken);
    }

    public async Task<IReadOnlyList<HouseholdMember>> GetByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.User)
            .Where(m => m.HouseholdId == householdId)
            .OrderBy(m => m.JoinedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetManagerCountAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(m => m.HouseholdId == householdId && m.Role == Roles.HouseholdManager, cancellationToken);
    }

    public async Task<int> GetMemberCountAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(m => m.HouseholdId == householdId, cancellationToken);
    }

    public async Task<bool> IsUserInAnyHouseholdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(m => m.UserId == userId, cancellationToken);
    }

    public async Task<HouseholdMember?> FindUnlinkedMemberAsync(Guid householdId, string fullName, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            m => m.HouseholdId == householdId && m.UserId == null && m.FullName.ToLower() == fullName.ToLower(),
            cancellationToken);
    }
}
