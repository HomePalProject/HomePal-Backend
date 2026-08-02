using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Pagination;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/measurement-units")]
public class MeasurementUnitsController : BaseApiController
{
    private readonly IMeasurementUnitService _measurementUnitService;

    public MeasurementUnitsController(IMeasurementUnitService measurementUnitService)
    {
        _measurementUnitService = measurementUnitService;
    }

    /// <summary>
    /// Get paginated list of measurement units with optional search query
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<MeasurementUnitResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMeasurementUnits([FromQuery] PaginationRequest request, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await _measurementUnitService.GetMeasurementUnitsAsync(request, search, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get measurement unit details by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<MeasurementUnitResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMeasurementUnitById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _measurementUnitService.GetMeasurementUnitByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new measurement unit (Admin or Manager only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<MeasurementUnitResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMeasurementUnit([FromBody] CreateMeasurementUnitRequest request, CancellationToken cancellationToken)
    {
        var result = await _measurementUnitService.CreateMeasurementUnitAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update a measurement unit by ID
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<MeasurementUnitResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMeasurementUnit(Guid id, [FromBody] UpdateMeasurementUnitRequest request, CancellationToken cancellationToken)
    {
        var result = await _measurementUnitService.UpdateMeasurementUnitAsync(id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a measurement unit by ID
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMeasurementUnit(Guid id, CancellationToken cancellationToken)
    {
        var result = await _measurementUnitService.DeleteMeasurementUnitAsync(id, cancellationToken);
        return HandleResult(result);
    }
}
