using System.ComponentModel;
using HomePal.Application.Features.ShoppingList.DTOs;
using HomePal.Application.Features.ShoppingList.Interfaces;
using HomePal.Infrastructure.AI.Common;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

/// <summary>
/// AI Agent Tool for managing household shopping list items via IShoppingListService.
/// </summary>
public class ShoppingListTools
{
    private readonly IShoppingListService _shoppingListService;
    private readonly AgentUserContext _userContext;

    public ShoppingListTools(
        IShoppingListService shoppingListService,
        AgentUserContext userContext)
    {
        _shoppingListService = shoppingListService;
        _userContext = userContext;
    }

    [Description("Gets all items currently on the user's household shopping list, including purchased status, quantities, prices, measuring units, categories, and associated meal plan.")]
    public async Task<object> GetShoppingListAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var result = await _shoppingListService.GetShoppingListAsync(userId, cancellationToken);
        if (!result.Success || result.Data == null)
            return new { success = false, error = result.Message };

        return new
        {
            success = true,
            totalItems = result.Data.Count,
            unpurchasedCount = result.Data.Count(i => !i.IsPurchased),
            items = result.Data
        };
    }

    [Description("Adds an item to the user's household shopping list. Before adding, call GetShoppingListAsync to check for duplicates — if the item already exists, use UpdateShoppingListItemAsync instead to merge quantities.")]
    public async Task<object> AddShoppingListItemAsync(
        [Description("The name of the item to add to the shopping list (e.g. 'Tomatoes', 'Olive Oil').")] string name,
        [Description("The quantity of the item (e.g. 2, 1.5). Default is 1.")] double quantity = 1,
        [Description("The portion count for recipes/meals. Default is 1.")] int portionCount = 1,
        [Description("Optional unique ID of the measuring unit. Retrieve from GetCategoriesAndUnitsAsync if needed. Never guess or fabricate this GUID.")] Guid? unitId = null,
        [Description("Optional unique ID of the product category. Retrieve from GetCategoriesAndUnitsAsync if needed. Never guess or fabricate this GUID.")] Guid? categoryId = null,
        [Description("Optional unique ID of the supermarket offer to link this item to. Use the 'id' field returned by SearchOffersAsync — passing it here links the discount to this shopping list item.")] Guid? offerId = null,
        [Description("Optional estimated or known price for the item.")] decimal? price = null,
        [Description("Optional notes or brand preferences.")] string? notes = null,
        [Description("Optional unique ID of the meal plan this item belongs to.")] Guid? mealPlanId = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var request = new CreateShoppingListItemRequest
        {
            Name = name?.Trim() ?? string.Empty,
            Quantity = quantity > 0 ? quantity : 1,
            PortionCount = portionCount > 0 ? portionCount : 1,
            Price = price,
            UnitId = unitId,
            CategoryId = categoryId,
            OfferId = offerId,
            MealPlanId = mealPlanId,
            Notes = notes
        };

        var result = await _shoppingListService.AddCustomItemAsync(userId, request, cancellationToken);
        if (!result.Success || result.Data == null)
            return new { success = false, error = result.Message };

        return new
        {
            success = true,
            message = "Shopping list item added successfully.",
            item = result.Data
        };
    }

    [Description("Updates an existing item on the shopping list (e.g., mark as purchased, change quantity, price, or attach a meal plan). The item is located by exact ID match if itemId is provided, or by exact then fuzzy name match using itemName.")]
    public async Task<object> UpdateShoppingListItemAsync(
        [Description("The name of the item to update (e.g. 'Tomatoes') if ID is not specified.")] string? itemName = null,
        [Description("The unique ID of the shopping list item to update (if known).")] Guid? itemId = null,
        [Description("Updated name for the item if renaming.")] string? newName = null,
        [Description("Updated quantity for the item.")] double? quantity = null,
        [Description("Updated portion count for the item.")] int? portionCount = null,
        [Description("Updated measuring unit ID.")] Guid? unitId = null,
        [Description("Updated product category ID.")] Guid? categoryId = null,
        [Description("Updated supermarket offer ID.")] Guid? offerId = null,
        [Description("Updated price for the item.")] decimal? price = null,
        [Description("Whether the item has been purchased (true/false).")] bool? isPurchased = null,
        [Description("Updated notes for the item.")] string? notes = null,
        [Description("Optional meal plan ID to associate with this item.")] Guid? mealPlanId = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var listResult = await _shoppingListService.GetShoppingListAsync(userId, cancellationToken);
        if (!listResult.Success || listResult.Data == null)
            return new { success = false, error = listResult.Message };

        var existing = itemId.HasValue
            ? listResult.Data.FirstOrDefault(i => i.Id == itemId.Value)
            : !string.IsNullOrWhiteSpace(itemName)
                ? listResult.Data.FirstOrDefault(i => i.Name.Trim().Equals(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                  ?? listResult.Data.FirstOrDefault(i => i.Name.Contains(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                : null;

        if (existing == null)
            return new { success = false, error = "Shopping list item not found." };

        var request = new UpdateShoppingListItemRequest
        {
            Name = !string.IsNullOrWhiteSpace(newName) ? newName.Trim() : existing.Name,
            Quantity = quantity.HasValue && quantity.Value > 0 ? quantity.Value : existing.Quantity,
            PortionCount = portionCount.HasValue && portionCount.Value > 0 ? portionCount.Value : existing.PortionCount,
            UnitId = unitId ?? existing.UnitId,
            CategoryId = categoryId ?? existing.CategoryId,
            OfferId = offerId ?? existing.OfferId,
            MealPlanId = mealPlanId ?? existing.MealPlanId,
            Price = price ?? existing.Price,
            IsPurchased = isPurchased ?? existing.IsPurchased,
            Notes = notes ?? existing.Notes
        };

        var updateResult = await _shoppingListService.UpdateItemAsync(userId, existing.Id, request, cancellationToken);
        if (!updateResult.Success || updateResult.Data == null)
            return new { success = false, error = updateResult.Message };

        return new
        {
            success = true,
            message = "Shopping list item updated successfully.",
            item = updateResult.Data
        };
    }

    [Description("Deletes an item from the user's household shopping list. The item is located by exact ID match if itemId is provided, or by exact then fuzzy name match using itemName.")]
    public async Task<object> DeleteShoppingListItemAsync(
        [Description("The name of the item to delete (if ID is not specified).")] string? itemName = null,
        [Description("The unique ID of the item to delete (if known).")] Guid? itemId = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var listResult = await _shoppingListService.GetShoppingListAsync(userId, cancellationToken);
        if (!listResult.Success || listResult.Data == null)
            return new { success = false, error = listResult.Message };

        var existing = itemId.HasValue
            ? listResult.Data.FirstOrDefault(i => i.Id == itemId.Value)
            : !string.IsNullOrWhiteSpace(itemName)
                ? listResult.Data.FirstOrDefault(i => i.Name.Trim().Equals(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                  ?? listResult.Data.FirstOrDefault(i => i.Name.Contains(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                : null;

        if (existing == null)
            return new { success = false, error = "Shopping list item not found." };

        var deleteResult = await _shoppingListService.DeleteItemAsync(userId, existing.Id, cancellationToken);
        if (!deleteResult.Success)
            return new { success = false, error = deleteResult.Message };

        return new
        {
            success = true,
            message = $"Item '{existing.Name}' removed from shopping list successfully."
        };
    }
}
