using System.ComponentModel;
using HomePal.Application.Features.Catalog.Interfaces;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

/// <summary>
/// AI Agent Tool for searching supermarket offers and discounts via hybrid search (vector similarity + full-text RRF).
/// </summary>
public class OfferSearchTools
{
    private readonly IOfferService _offerService;

    public OfferSearchTools(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [Description("Searches active supermarket offers, catalog items, and grocery deals using semantic vector search. Useful for price comparison, checking supermarket discounts, and finding cheap ingredients. IMPORTANT: Store the 'id' field from each returned offer and pass it as the 'offerId' parameter when calling AddShoppingListItemAsync — this links the discount to the shopping list item.")]
    public async Task<object> SearchOffersAsync(
        [Description("The search query describing the product, ingredient, or item (e.g. 'fresh whole milk', 'extra virgin olive oil', 'chicken breast', 'cheddar cheese').")] string query,
        [Description("The maximum number of offer results to return (1-10). Default is 5.")] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var clampedLimit = Math.Clamp(limit, 1, 10);
        var result = await _offerService.SearchOffersAsync(query, clampedLimit, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            return new
            {
                success = false,
                query,
                error = result.Message,
                totalResults = 0,
                offers = Array.Empty<object>()
            };
        }

        return new
        {
            success = true,
            query,
            totalResults = result.Data.Count,
            offers = result.Data.Select(o => new
            {
                id = o.Id,
                name = o.Name,
                description = o.Description,
                supermarket = o.SupermarketName,
                category = o.CategoryName,
                originalPrice = o.OriginalPrice,
                discountedPrice = o.DiscountedPrice,
                unitId=o.UnitId,
                categoryId=o.CategoryId,
                quantity = o.Quantity,
                unit = o.UnitSymbol ?? o.UnitName,
                sourceUrl = o.SourceUrl,
                validFrom = o.ValidFrom,
                validTo = o.ValidTo
            })
        };
    }
}
