using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IPreferenceCategoryRepository : IRepository<PreferenceCategory>
{
    Task<PreferenceCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<PreferenceCategory?> GetByIdWithPreferencesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PreferenceCategory>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
