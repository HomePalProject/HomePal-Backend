using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagement.Interfaces;

public interface IMeasurementUnitService
{
    Task<Result<PaginatedList<MeasurementUnitResponse>>> GetMeasurementUnitsAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default);
    Task<Result<MeasurementUnitResponse>> GetMeasurementUnitByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<MeasurementUnitResponse>> CreateMeasurementUnitAsync(CreateMeasurementUnitRequest request, CancellationToken cancellationToken = default);
    Task<Result<MeasurementUnitResponse>> UpdateMeasurementUnitAsync(Guid id, UpdateMeasurementUnitRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteMeasurementUnitAsync(Guid id, CancellationToken cancellationToken = default);
}
