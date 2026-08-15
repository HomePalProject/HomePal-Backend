using HomePal.Application.Features.Locations.DTOs;
using HomePal.Application.Features.Locations.Interfaces;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[AllowAnonymous]
[Route("api/locations")]
public class LocationsController : BaseApiController
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    /// <summary>
    /// Get all Egyptian governorates
    /// </summary>
    [HttpGet("governorates")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<GovernorateResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGovernorates([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetGovernoratesAsync(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single governorate by ID with all its cities
    /// </summary>
    [HttpGet("governorates/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GovernorateDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGovernorateById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetGovernorateByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single governorate by 3-letter code (e.g. CAI, ALX) with all its cities
    /// </summary>
    [HttpGet("governorates/code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<GovernorateDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGovernorateByCode(string code, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetGovernorateByCodeAsync(code, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all cities in a specific governorate
    /// </summary>
    [HttpGet("governorates/{governorateId:guid}/cities")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<CityResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCitiesByGovernorate(Guid governorateId, [FromQuery] string? query, CancellationToken cancellationToken)
    {
        var request = new CityQueryRequest
        {
            GovernorateId = governorateId,
            Query = query
        };
        var result = await _locationService.GetCitiesAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all cities with optional filtering by governorate ID, governorate code, or search term
    /// </summary>
    [HttpGet("cities")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<CityResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCities([FromQuery] CityQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetCitiesAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single city by ID
    /// </summary>
    [HttpGet("cities/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCityById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _locationService.GetCityByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }
}
