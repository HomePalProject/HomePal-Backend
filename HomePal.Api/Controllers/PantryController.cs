using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/pantry")]
public class PantryController : BaseApiController
{
    private readonly IPantryItemService _pantryItemService;

    public PantryController(IPantryItemService pantryItemService)
    {
        _pantryItemService = pantryItemService;
    }

    /// <summary>
    /// Get all pantry items for current user's household
    /// </summary>
    [HttpGet("items")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PantryItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPantryItems(CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.GetPantryItemsAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get details of a specific pantry item
    /// </summary>
    [HttpGet("items/{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PantryItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPantryItemById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.GetPantryItemByIdAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new pantry item
    /// </summary>
    [HttpPost("items")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PantryItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePantryItem([FromBody] CreatePantryItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.CreatePantryItemAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing pantry item
    /// </summary>
    [HttpPut("items/{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PantryItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePantryItem([FromRoute] Guid id, [FromBody] UpdatePantryItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.UpdatePantryItemAsync(CurrentUserId, id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update/replace entire pantry items list for household
    /// </summary>
    [HttpPut("items")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PantryItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEntirePantryItems([FromBody] UpdateEntirePantryItemsRequest request, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.UpdateEntirePantryItemsAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a pantry item
    /// </summary>
    [HttpDelete("items/{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePantryItem([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.DeletePantryItemAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Pantry camera scan (returns dummy scanned items without saving)
    /// </summary>
    [HttpPost("scan")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PantryScanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ScanPantryCamera(CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.ScanPantryCameraAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Pantry bulk add endpoint (saves provided items)
    /// </summary>
    [HttpPost("bulk-add")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PantryItemResponse>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BulkAddPantryItems([FromBody] BulkAddPantryItemsRequest request, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.BulkAddPantryItemsAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }
}
