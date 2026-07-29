using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/preferences")]
public class PreferencesController : BaseApiController
{
    private readonly IPreferenceService _preferenceService;

    public PreferencesController(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    /// <summary>
    /// Get all available system preferences
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PreferenceResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPreferences([FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.GetAllPreferencesAsync(categoryId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Search system preferences by query term
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PreferenceResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchPreferences([FromQuery] string? query, [FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.SearchPreferencesAsync(query, categoryId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single system preference by ID
    /// </summary>
    [HttpGet("{preferenceId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PreferenceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPreferenceById(Guid preferenceId, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.GetPreferenceByIdAsync(preferenceId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new system preference (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PreferenceResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePreference([FromBody] AddPreferenceRequest request, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.CreatePreferenceAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing system preference (Admin only)
    /// </summary>
    [HttpPut("{preferenceId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PreferenceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePreference(Guid preferenceId, [FromBody] UpdatePreferenceRequest request, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.UpdatePreferenceAsync(CurrentUserId, preferenceId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a system preference (Admin only)
    /// </summary>
    [HttpDelete("{preferenceId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePreference(Guid preferenceId, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.DeletePreferenceAsync(CurrentUserId, preferenceId, cancellationToken);
        return HandleResult(result);
    }
}
