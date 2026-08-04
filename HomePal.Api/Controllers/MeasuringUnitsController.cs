using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/units")]
public class MeasuringUnitsController : BaseApiController
{
    private readonly IMeasuringUnitService _measuringUnitService;

    public MeasuringUnitsController(IMeasuringUnitService measuringUnitService)
    {
        _measuringUnitService = measuringUnitService;
    }

    /// <summary>
    /// Get all available measuring units
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<MeasuringUnitResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMeasuringUnits([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var result = await _measuringUnitService.GetAllAsync(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Search measuring units by query term
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<MeasuringUnitResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchMeasuringUnits([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var result = await _measuringUnitService.GetAllAsync(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single measuring unit by ID
    /// </summary>
    [HttpGet("{measuringUnitId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<MeasuringUnitResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMeasuringUnitById(Guid measuringUnitId, CancellationToken cancellationToken)
    {
        var result = await _measuringUnitService.GetByIdAsync(measuringUnitId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new measuring unit (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<MeasuringUnitResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateMeasuringUnit([FromBody] CreateMeasuringUnitRequest request, CancellationToken cancellationToken)
    {
        var result = await _measuringUnitService.CreateAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing measuring unit (Admin only)
    /// </summary>
    [HttpPut("{measuringUnitId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<MeasuringUnitResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMeasuringUnit(Guid measuringUnitId, [FromBody] UpdateMeasuringUnitRequest request, CancellationToken cancellationToken)
    {
        var result = await _measuringUnitService.UpdateAsync(measuringUnitId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a measuring unit (Admin only)
    /// </summary>
    [HttpDelete("{measuringUnitId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMeasuringUnit(Guid measuringUnitId, CancellationToken cancellationToken)
    {
        var result = await _measuringUnitService.DeleteAsync(measuringUnitId, cancellationToken);
        return HandleResult(result);
    }
}
