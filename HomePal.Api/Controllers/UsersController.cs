using HomePal.Application.Features.UserManagement.DTOs;
using HomePal.Application.Features.UserManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Pagination;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize(Roles = Roles.Admin)]
[Route("api/users")]
public class UsersController : BaseApiController
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    /// <summary>
    /// Get paginated list of users (filterable by role and search term)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] UserQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.GetUsersAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get user details by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.GetUserByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new Admin user
    /// </summary>
    [HttpPost("admins")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status499ClientClosedRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddAdmin([FromBody] CreateAdminRequest request, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.AddAdminAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update Admin profile (excluding username)
    /// </summary>
    [HttpPut("admins/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] UpdateAdminRequest request, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.UpdateAdminAsync(id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a user account (cannot delete default admin)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.DeleteUserAsync(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Deactivate a user account (cannot deactivate default admin)
    /// </summary>
    [HttpPut("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAccount(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.DeactivateAccountAsync(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete an admin user account (cannot delete admin with username admin)
    /// </summary>
    [HttpDelete("admins/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAdmin(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.DeleteAdminAsync(id, cancellationToken);
        return HandleResult(result);
    }
}
