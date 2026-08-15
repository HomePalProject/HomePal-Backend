using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Locations.Interfaces;

public interface ICityRepository : IRepository<City>
{
    Task<IReadOnlyList<City>> SearchAsync(Guid? governorateId = null, string? governorateCode = null, string? query = null, CancellationToken cancellationToken = default);
    Task<City?> GetByIdWithGovernorateAsync(Guid id, CancellationToken cancellationToken = default);
}
