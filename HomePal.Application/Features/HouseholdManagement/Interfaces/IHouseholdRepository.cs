using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IHouseholdRepository : IRepository<Household>
{
    Task<Household?> GetByIdWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Household?> GetByMemberUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
