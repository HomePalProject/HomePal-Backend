using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Locations.DTOs;
using HomePal.Application.Features.Locations.Interfaces;
using HomePal.Application.Features.Locations.Mappers;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Locations.Services;

public class LocationService : ILocationService
{
    private readonly IUnitOfWork _unitOfWork;

    public LocationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyCollection<GovernorateResponse>>> GetGovernoratesAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        var governorates = await _unitOfWork.Governorates.SearchAsync(query, cancellationToken);
        var responses = governorates.Select(g => g.ToResponse()).ToList();
        return Result<IReadOnlyCollection<GovernorateResponse>>.Ok(responses, SuccessMessages.Locations.GetAllGovernorates);
    }

    public async Task<Result<GovernorateDetailResponse>> GetGovernorateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var governorate = await _unitOfWork.Governorates.GetByIdWithCitiesAsync(id, cancellationToken);
        if (governorate == null)
        {
            return Result<GovernorateDetailResponse>.Fail(ErrorMessages.Locations.GovernorateNotFound, ResultStatus.NotFound);
        }

        return Result<GovernorateDetailResponse>.Ok(governorate.ToDetailResponse(), SuccessMessages.Locations.GetGovernorate);
    }

    public async Task<Result<GovernorateDetailResponse>> GetGovernorateByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<GovernorateDetailResponse>.Fail(ErrorMessages.Locations.GovernorateNotFound, ResultStatus.NotFound);
        }

        var governorate = await _unitOfWork.Governorates.GetByCodeAsync(code, includeCities: true, cancellationToken);
        if (governorate == null)
        {
            return Result<GovernorateDetailResponse>.Fail(ErrorMessages.Locations.GovernorateNotFound, ResultStatus.NotFound);
        }

        return Result<GovernorateDetailResponse>.Ok(governorate.ToDetailResponse(), SuccessMessages.Locations.GetGovernorate);
    }

    public async Task<Result<IReadOnlyCollection<CityResponse>>> GetCitiesAsync(CityQueryRequest? request = null, CancellationToken cancellationToken = default)
    {
        var cities = await _unitOfWork.Cities.SearchAsync(
            request?.GovernorateId,
            request?.GovernorateCode,
            request?.Query,
            cancellationToken);

        var responses = cities.Select(c => c.ToResponse()).ToList();
        return Result<IReadOnlyCollection<CityResponse>>.Ok(responses, SuccessMessages.Locations.GetAllCities);
    }

    public async Task<Result<CityResponse>> GetCityByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var city = await _unitOfWork.Cities.GetByIdWithGovernorateAsync(id, cancellationToken);
        if (city == null)
        {
            return Result<CityResponse>.Fail(ErrorMessages.Locations.CityNotFound, ResultStatus.NotFound);
        }

        return Result<CityResponse>.Ok(city.ToResponse(), SuccessMessages.Locations.GetCity);
    }
}
