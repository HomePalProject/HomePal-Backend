using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IPreferenceRepository : IRepository<Preference>
{
    Task<Preference?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Preference>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Preference>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Preference>> SearchAsync(string query, Guid? categoryId = null, CancellationToken cancellationToken = default);
}
