using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/pantry/items")]
public class PantryItemsController : BaseApiController
{
    private readonly IPantryItemService _pantryItemService;

    public PantryItemsController(IPantryItemService pantryItemService)
    {
        _pantryItemService = pantryItemService;
    }

    /// <summary>
    /// Get all Pantry Items for the authenticated user's Household
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PantryItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPantryItems(CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.GetPantryItemsAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single Pantry Item details by Item ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PantryItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPantryItemById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.GetPantryItemByIdAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Add a new Pantry Item to the household Pantry
    /// </summary>
    [HttpPost]
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
    /// Update an existing Pantry Item
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PantryItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePantryItem(Guid id, [FromBody] UpdatePantryItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.UpdatePantryItemAsync(CurrentUserId, id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a Pantry Item from the household Pantry
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePantryItem(Guid id, CancellationToken cancellationToken)
    {
        var result = await _pantryItemService.DeletePantryItemAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }
}
