using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/households/invitations")]
public class HouseholdInvitationsController : BaseApiController
{
    private readonly IHouseholdInvitationService _invitationService;

    public HouseholdInvitationsController(IHouseholdInvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    /// <summary>
    /// Send a household invitation to a user by Username or Email (Household Manager only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<HouseholdInvitationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendInvitation([FromBody] SendInvitationRequest request, CancellationToken cancellationToken)
    {
        var result = await _invitationService.SendInvitationAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all pending invitations received by the current logged-in user
    /// </summary>
    [HttpGet("my-invitations")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<HouseholdInvitationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyInvitations(CancellationToken cancellationToken)
    {
        var result = await _invitationService.GetMyInvitationsAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all invitations sent for the current household (Household Manager only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<HouseholdInvitationResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetHouseholdInvitations(CancellationToken cancellationToken)
    {
        var result = await _invitationService.GetHouseholdInvitationsAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cancel a sent pending invitation (Household Manager only)
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CancelInvitation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invitationService.CancelInvitationAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Accept a household invitation
    /// </summary>
    [HttpPost("{id:guid}/accept")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AcceptInvitation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invitationService.AcceptInvitationAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Decline a household invitation
    /// </summary>
    [HttpPost("{id:guid}/decline")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeclineInvitation(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invitationService.DeclineInvitationAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }
}
