using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.PantryManagement.Interfaces;

public interface IPantryScannerService
{
    Task<Result<PantryScanResponse>> ScanPantryImageAsync(
        Stream imageStream,
        string contentType,
        IReadOnlyList<MeasuringUnit> availableUnits,
        IReadOnlyList<ProductCategory> availableCategories,
        CancellationToken cancellationToken = default);
}
