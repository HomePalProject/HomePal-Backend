using HomePal.Application.Features.ShoppingList.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.ShoppingList.Mappers;

public static class ShoppingListItemMapper
{
    public static ShoppingListItemResponse ToResponse(this ShoppingListItem item, string? culture = null)
    {
        var supermarket = item.Offer?.Supermarket;
        var portionCount = item.PortionCount > 0 ? item.PortionCount : 1;

        return new ShoppingListItemResponse
        {
            Id = item.Id,
            ShoppingListId = item.ShoppingListId,
            Name = item.Name,
            Quantity = item.Quantity,
            PortionCount = portionCount,
            Price = item.Price,
            TotalPrice = item.Price.HasValue ? item.Price.Value * portionCount : null,
            UnitId = item.MeasuringUnitId,
            UnitName = item.MeasuringUnit?.Name.Get(culture),
            UnitSymbol = item.MeasuringUnit?.Symbol?.Get(culture),
            CategoryId = item.CategoryId,
            CategoryName = item.Category?.Name.Get(culture),
            OfferId = item.OfferId,
            OfferImagePath = item.Offer?.ImagePath,
            SupermarketName = supermarket?.Name.Get(culture),
            SupermarketLogoPath = supermarket?.LogoPath,
            MealPlanId = item.MealPlanId,
            IsPurchased = item.IsPurchased,
            Notes = item.Notes,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    public static ShoppingListItem ToEntity(this CreateShoppingListItemRequest request, Guid shoppingListId)
    {
        return new ShoppingListItem
        {
            Id = Guid.NewGuid(),
            ShoppingListId = shoppingListId,
            Name = request.Name.Trim(),
            Quantity = request.Quantity > 0 ? request.Quantity : 1,
            PortionCount = request.PortionCount > 0 ? request.PortionCount : 1,
            Price = request.Price,
            MeasuringUnitId = request.UnitId,
            CategoryId = request.CategoryId,
            OfferId = request.OfferId,
            MealPlanId = request.MealPlanId,
            Notes = request.Notes,
            IsPurchased = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static ShoppingListItem ToEntity(this Offer offer, Guid shoppingListId, double? customQuantity = null, int? portionCount = null, string? notes = null, string? culture = null)
    {
        var offerName = offer.Name.Get(culture);
        return new ShoppingListItem
        {
            Id = Guid.NewGuid(),
            ShoppingListId = shoppingListId,
            Name = !string.IsNullOrWhiteSpace(offerName) ? offerName : offer.Name.Get("en"),
            Quantity = customQuantity.HasValue && customQuantity.Value > 0 ? customQuantity.Value : (offer.Quantity > 0 ? offer.Quantity : 1),
            PortionCount = portionCount.HasValue && portionCount.Value > 0 ? portionCount.Value : 1,
            Price = offer.DiscountedPrice > 0 ? offer.DiscountedPrice : offer.OriginalPrice,
            MeasuringUnitId = offer.UnitId,
            CategoryId = offer.CategoryId,
            OfferId = offer.Id,
            Notes = notes,
            IsPurchased = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this ShoppingListItem item, UpdateShoppingListItemRequest request)
    {
        item.Name = request.Name.Trim();
        item.Quantity = request.Quantity > 0 ? request.Quantity : 1;
        item.PortionCount = request.PortionCount > 0 ? request.PortionCount : 1;
        item.Price = request.Price;
        item.MeasuringUnitId = request.UnitId;
        item.CategoryId = request.CategoryId;
        item.MealPlanId = request.MealPlanId;
        item.IsPurchased = request.IsPurchased;
        item.Notes = request.Notes;
        item.OfferId = request.OfferId ?? item.OfferId;
        item.UpdatedAt = DateTime.UtcNow;
    }
}
