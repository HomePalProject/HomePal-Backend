using HomePal.Application.Features.Locations.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Locations.Interfaces;

public interface ILocationService
{
    Task<Result<IReadOnlyCollection<GovernorateResponse>>> GetGovernoratesAsync(string? query = null, CancellationToken cancellationToken = default);
    Task<Result<GovernorateDetailResponse>> GetGovernorateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<GovernorateDetailResponse>> GetGovernorateByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<CityResponse>>> GetCitiesAsync(CityQueryRequest? request = null, CancellationToken cancellationToken = default);
    Task<Result<CityResponse>> GetCityByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
