using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/households/members")]
public class HouseholdMembersController : BaseApiController
{
    private readonly IHouseholdMemberService _memberService;

    public HouseholdMembersController(IHouseholdMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>
    /// Get all members of the current user's household
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<HouseholdMemberResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMembers(CancellationToken cancellationToken)
    {
        var result = await _memberService.GetHouseholdMembersAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single household member details by member ID
    /// </summary>
    [HttpGet("{memberId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<HouseholdMemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMemberById(Guid memberId, CancellationToken cancellationToken)
    {
        var result = await _memberService.GetMemberByIdAsync(CurrentUserId, memberId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Add an offline (non-registered) family member (Household Manager only)
    /// </summary>
    [HttpPost("offline")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<HouseholdMemberResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddOfflineMember([FromBody] AddOfflineMemberRequest request, CancellationToken cancellationToken)
    {
        var result = await _memberService.AddOfflineMemberAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update details of a household member (Household Manager only)
    /// </summary>
    [HttpPut("{memberId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<HouseholdMemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMember(Guid memberId, [FromBody] UpdateMemberRequest request, CancellationToken cancellationToken)
    {
        var result = await _memberService.UpdateMemberAsync(CurrentUserId, memberId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Remove a member from the household or leave the household (Manager or Self)
    /// </summary>
    [HttpDelete("{memberId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMember(Guid memberId, CancellationToken cancellationToken)
    {
        var result = await _memberService.RemoveMemberAsync(CurrentUserId, memberId, cancellationToken);
        return HandleResult(result);
    }
}
