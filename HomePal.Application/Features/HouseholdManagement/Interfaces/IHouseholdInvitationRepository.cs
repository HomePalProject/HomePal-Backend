using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IHouseholdInvitationRepository
{
    Task<HouseholdInvitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HouseholdInvitation?> GetByIdAndHouseholdIdAsync(Guid id, Guid householdId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HouseholdInvitation>> GetPendingByEmailOrUsernameAsync(string? email, string? userName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HouseholdInvitation>> GetByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default);
    Task<bool> HasPendingInvitationAsync(Guid householdId, string input, CancellationToken cancellationToken = default);
    Task AddAsync(HouseholdInvitation invitation, CancellationToken cancellationToken = default);
    void Update(HouseholdInvitation invitation);
}
