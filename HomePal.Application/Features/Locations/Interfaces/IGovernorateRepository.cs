using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Locations.Interfaces;

public interface IGovernorateRepository : IRepository<Governorate>
{
    Task<IReadOnlyList<Governorate>> SearchAsync(string? query, CancellationToken cancellationToken = default);
    Task<Governorate?> GetByIdWithCitiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Governorate?> GetByCodeAsync(string code, bool includeCities = false, CancellationToken cancellationToken = default);
}
