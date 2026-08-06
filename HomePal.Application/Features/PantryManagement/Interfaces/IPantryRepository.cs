using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.PantryManagement.Interfaces;

public interface IPantryRepository : IRepository<Pantry>
{
    Task<Pantry?> GetByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default);
    Task<Pantry?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
