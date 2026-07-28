using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
public class HouseholdsController : BaseApiController
{
    private readonly IHouseholdService _householdService;

    public HouseholdsController(IHouseholdService householdService)
    {
        _householdService = householdService;
    }

    /// <summary>
    /// Create a new household (User becomes Household Manager)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<HouseholdResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHousehold([FromBody] CreateHouseholdRequest request, CancellationToken cancellationToken)
    {
        var result = await _householdService.CreateHouseholdAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get details of current user's household
    /// </summary>
    [HttpGet("my-household")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<HouseholdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyHousehold(CancellationToken cancellationToken)
    {
        var result = await _householdService.GetMyHouseholdAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update household details (Household Manager only)
    /// </summary>
    [HttpPut]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<HouseholdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHousehold([FromBody] UpdateHouseholdRequest request, CancellationToken cancellationToken)
    {
        var result = await _householdService.UpdateHouseholdAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete the household and disband all members (Household Manager only)
    /// </summary>
    [HttpDelete]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHousehold(CancellationToken cancellationToken)
    {
        var result = await _householdService.DeleteHouseholdAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }
}
