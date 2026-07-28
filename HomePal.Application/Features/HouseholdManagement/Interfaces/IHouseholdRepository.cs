using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IHouseholdRepository
{
    Task<Household?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Household?> GetByMemberUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Household household, CancellationToken cancellationToken = default);
    void Update(Household household);
    void Remove(Household household);
}
