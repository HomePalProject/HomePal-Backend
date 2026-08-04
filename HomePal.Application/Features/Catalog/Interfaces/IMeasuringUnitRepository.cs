using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IMeasuringUnitRepository : IRepository<MeasuringUnit>
{
    Task<IReadOnlyList<MeasuringUnit>> SearchAsync(string? query, CancellationToken cancellationToken = default);
}
