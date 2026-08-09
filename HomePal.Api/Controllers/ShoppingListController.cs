using HomePal.Application.Features.ShoppingList.DTOs;
using HomePal.Application.Features.ShoppingList.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/shopping-list")]
public class ShoppingListController : BaseApiController
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    /// <summary>
    /// Get the current household's shared shopping list
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<List<ShoppingListItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShoppingList(CancellationToken cancellationToken)
    {
        var result = await _shoppingListService.GetShoppingListAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Add a custom user entry item to the shopping list
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<ShoppingListItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCustomItem([FromBody] CreateShoppingListItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _shoppingListService.AddCustomItemAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Add an item directly from a supermarket offer to the shopping list
    /// </summary>
    [HttpPost("from-offer/{offerId}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<ShoppingListItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddFromOffer(Guid offerId, [FromBody] AddFromOfferRequest request, CancellationToken cancellationToken)
    {
        request.OfferId = offerId;
        var result = await _shoppingListService.AddFromOfferAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing item's name, quantity, unit, price, notes, or purchased status
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<ShoppingListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateShoppingListItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _shoppingListService.UpdateItemAsync(CurrentUserId, id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Toggle the purchased / checked status of a shopping list item
    /// </summary>
    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<ShoppingListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TogglePurchased(Guid id, CancellationToken cancellationToken)
    {
        var result = await _shoppingListService.TogglePurchasedAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a single item from the shopping list
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(Guid id, CancellationToken cancellationToken)
    {
        var result = await _shoppingListService.DeleteItemAsync(CurrentUserId, id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Clear all purchased / checked-off items from the household shopping list
    /// </summary>
    [HttpDelete("purchased")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearPurchased(CancellationToken cancellationToken)
    {
        var result = await _shoppingListService.ClearPurchasedAsync(CurrentUserId, cancellationToken);
        return HandleResult(result);
    }
}
