using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IHouseholdMemberRepository : IRepository<HouseholdMember>
{
    Task<HouseholdMember?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<HouseholdMember?> GetByIdAndHouseholdIdAsync(Guid id, Guid householdId, CancellationToken cancellationToken = default);
    Task<HouseholdMember?> GetByIdWithPreferencesAsync(Guid id, Guid householdId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HouseholdMember>> GetByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default);
    Task<int> GetManagerCountAsync(Guid householdId, CancellationToken cancellationToken = default);
    Task<int> GetMemberCountAsync(Guid householdId, CancellationToken cancellationToken = default);
    Task<bool> IsUserInAnyHouseholdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<HouseholdMember?> FindUnlinkedMemberAsync(Guid householdId, string fullName, CancellationToken cancellationToken = default);
}
