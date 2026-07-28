using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Domain.Enums;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class HouseholdInvitationRepository : Repository<HouseholdInvitation>, IHouseholdInvitationRepository
{
    public HouseholdInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<HouseholdInvitation?> GetByIdAndHouseholdIdAsync(Guid id, Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Household)
            .Include(i => i.InvitedBy)
            .FirstOrDefaultAsync(i => i.Id == id && i.HouseholdId == householdId, cancellationToken);
    }

    public async Task<IReadOnlyList<HouseholdInvitation>> GetPendingByEmailOrUsernameAsync(string? email, string? userName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Household)
            .Include(i => i.InvitedBy)
            .Where(i => i.Status == InvitationStatus.Pending
                        && ((email != null && i.InvitedEmail == email) || (userName != null && i.InvitedUserName == userName)))
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<HouseholdInvitation>> GetByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Household)
            .Include(i => i.InvitedBy)
            .Where(i => i.HouseholdId == householdId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPendingInvitationAsync(Guid householdId, string input, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            i => i.HouseholdId == householdId 
              && i.Status == InvitationStatus.Pending 
              && (i.InvitedEmail == input || i.InvitedUserName == input),
            cancellationToken);
    }
}
