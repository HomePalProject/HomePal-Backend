using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/households/members/{memberId:guid}/preferences")]
public class MemberPreferencesController : BaseApiController
{
    private readonly IMemberPreferenceService _preferenceService;

    public MemberPreferencesController(IMemberPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    /// <summary>
    /// Get preferences assigned to a specific household member
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PreferenceResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberPreferences(Guid memberId, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.GetMemberPreferencesAsync(CurrentUserId, memberId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Set / update preferences assigned to a household member (Member Self or Household Manager)
    /// </summary>
    [HttpPut]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PreferenceResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetMemberPreferences(Guid memberId, [FromBody] AssignPreferencesRequest request, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.SetMemberPreferencesAsync(CurrentUserId, memberId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Remove a preference from a household member (Member Self or Household Manager)
    /// </summary>
    [HttpDelete("{preferenceId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMemberPreference(Guid memberId, Guid preferenceId, CancellationToken cancellationToken)
    {
        var result = await _preferenceService.RemoveMemberPreferenceAsync(CurrentUserId, memberId, preferenceId, cancellationToken);
        return HandleResult(result);
    }
}
