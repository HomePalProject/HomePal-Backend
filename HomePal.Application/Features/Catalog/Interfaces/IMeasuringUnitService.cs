using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IMeasuringUnitService
{
    Task<Result<IReadOnlyCollection<MeasuringUnitResponse>>> GetAllAsync(string? query = null, CancellationToken cancellationToken = default);
    Task<Result<MeasuringUnitResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<MeasuringUnitResponse>> CreateAsync(CreateMeasuringUnitRequest request, CancellationToken cancellationToken = default);
    Task<Result<MeasuringUnitResponse>> UpdateAsync(Guid id, UpdateMeasuringUnitRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
