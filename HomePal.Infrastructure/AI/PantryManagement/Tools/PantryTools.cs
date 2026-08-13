using System.ComponentModel;
using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Infrastructure.AI.Common;

namespace HomePal.Infrastructure.AI.PantryManagement.Tools;

/// <summary>
/// AI Agent Tool for querying and updating pantry inventory via IPantryItemService.
/// </summary>
public class PantryTools
{
    private readonly IPantryItemService _pantryItemService;
    private readonly AgentUserContext _userContext;

    public PantryTools(
        IPantryItemService pantryItemService,
        AgentUserContext userContext)
    {
        _pantryItemService = pantryItemService;
        _userContext = userContext;
    }

    [Description("Gets the list of all food items currently in the user's household pantry with their names, quantities, measuring units, categories, and expiration dates.")]
    public async Task<object> GetPantryAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var result = await _pantryItemService.GetPantryItemsAsync(userId, cancellationToken);
        if (!result.Success || result.Data == null)
            return new { success = false, error = result.Message };

        return new
        {
            success = true,
            totalItems = result.Data.Count,
            items = result.Data
        };
    }

    [Description("Adds a new item to the user's household pantry with specified quantity, measuring unit ID, category ID, and optional expiration date.")]
    public async Task<object> AddPantryItemAsync(
        [Description("The name of the pantry item to add (e.g. 'Milk', 'Flour', 'Eggs').")] string name,
        [Description("The quantity of the item (e.g. 2, 1.5). Default is 1.")] decimal quantity = 1,
        [Description("The unique ID of the measuring unit.")] Guid unitId = default,
        [Description("The unique ID of the product category.")] Guid categoryId = default,
        [Description("Optional expiration date for the item (e.g. '2026-08-30').")] DateTime? expireDate = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var request = new CreatePantryItemRequest
        {
            Name = name?.Trim() ?? string.Empty,
            Quantity = quantity > 0 ? quantity : 1,
            MeasuringUnitId = unitId,
            CategoryId = categoryId,
            ExpireDate = expireDate
        };

        var result = await _pantryItemService.CreatePantryItemAsync(userId, request, cancellationToken);
        if (!result.Success || result.Data == null)
            return new { success = false, error = result.Message };

        return new
        {
            success = true,
            message = "Pantry item added successfully.",
            item = result.Data
        };
    }

    [Description("Updates a pantry item in the user's household pantry (e.g. quantity, unitId, categoryId, expiration date, or name).")]
    public async Task<object> UpdatePantryAsync(
        [Description("The name of the item to update (e.g. 'Milk', 'Eggs') if ID is not specified.")] string? itemName = null,
        [Description("The unique ID of the pantry item to update (if known).")] Guid? itemId = null,
        [Description("The updated quantity for the item (e.g. 3, 1.5).")] decimal? quantity = null,
        [Description("Updated measuring unit ID.")] Guid? unitId = null,
        [Description("Updated product category ID.")] Guid? categoryId = null,
        [Description("The updated expiration date (e.g. '2026-08-25').")] DateTime? expireDate = null,
        [Description("The updated name for the item if renaming.")] string? newName = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var itemsResult = await _pantryItemService.GetPantryItemsAsync(userId, cancellationToken);
        if (!itemsResult.Success || itemsResult.Data == null)
            return new { success = false, error = itemsResult.Message };

        var existing = itemId.HasValue
            ? itemsResult.Data.FirstOrDefault(i => i.Id == itemId.Value)
            : !string.IsNullOrWhiteSpace(itemName)
                ? itemsResult.Data.FirstOrDefault(i => i.Name.Trim().Equals(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                  ?? itemsResult.Data.FirstOrDefault(i => i.Name.Contains(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                : null;

        if (existing == null)
            return new { success = false, error = "Pantry item not found." };

        var request = new UpdatePantryItemRequest
        {
            Name = !string.IsNullOrWhiteSpace(newName) ? newName.Trim() : existing.Name,
            Quantity = quantity.HasValue && quantity.Value >= 0 ? quantity.Value : existing.Quantity,
            MeasuringUnitId = unitId ?? existing.MeasuringUnitId,
            CategoryId = categoryId ?? existing.CategoryId,
            ExpireDate = expireDate ?? existing.ExpireDate
        };

        var updateResult = await _pantryItemService.UpdatePantryItemAsync(userId, existing.Id, request, cancellationToken);
        if (!updateResult.Success || updateResult.Data == null)
            return new { success = false, error = updateResult.Message };

        return new
        {
            success = true,
            message = "Pantry item updated successfully.",
            item = updateResult.Data
        };
    }

    [Description("Deletes an item from the user's household pantry.")]
    public async Task<object> DeletePantryItemAsync(
        [Description("The name of the item to delete (if ID is not specified).")] string? itemName = null,
        [Description("The unique ID of the item to delete (if known).")] Guid? itemId = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == Guid.Empty)
            return new { success = false, error = "User is not authenticated." };

        var itemsResult = await _pantryItemService.GetPantryItemsAsync(userId, cancellationToken);
        if (!itemsResult.Success || itemsResult.Data == null)
            return new { success = false, error = itemsResult.Message };

        var existing = itemId.HasValue
            ? itemsResult.Data.FirstOrDefault(i => i.Id == itemId.Value)
            : !string.IsNullOrWhiteSpace(itemName)
                ? itemsResult.Data.FirstOrDefault(i => i.Name.Trim().Equals(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                  ?? itemsResult.Data.FirstOrDefault(i => i.Name.Contains(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                : null;

        if (existing == null)
            return new { success = false, error = "Pantry item not found." };

        var deleteResult = await _pantryItemService.DeletePantryItemAsync(userId, existing.Id, cancellationToken);
        if (!deleteResult.Success)
            return new { success = false, error = deleteResult.Message };

        return new
        {
            success = true,
            message = $"Item '{existing.Name}' removed from pantry successfully."
        };
    }
}
