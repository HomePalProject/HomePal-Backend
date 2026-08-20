using System.ComponentModel;
using HomePal.Application.Features.MealPlanning.Interfaces;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

/// <summary>
/// AI Agent Tool for searching ingredients via MongoDB Atlas Vector Search.
/// </summary>
public class IngredientSearchTools
{
    private readonly IIngredientSearchService _ingredientSearchService;

    public IngredientSearchTools(IIngredientSearchService ingredientSearchService)
    {
        _ingredientSearchService = ingredientSearchService;
    }

    [Description("Searches the ingredient knowledge base using semantic vector search for food ingredients, culinary properties, nutritional facts, and ingredient specifications.")]
    public async Task<object> SearchIngredientsAsync(
        [Description("The ingredient search query (e.g. 'almond flour', 'olive oil', 'greek yogurt substitute', 'chia seeds'). If no results are returned, broaden the query or try a common alternative name for the ingredient.")] string query,
        [Description("The maximum number of ingredient results to return (1-10). Default is 5.")] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var clampedLimit = Math.Clamp(limit, 1, 10);
        var results = await _ingredientSearchService.SearchAsync(query, clampedLimit, cancellationToken);

        return new
        {
            success = true,
            query,
            totalResults = results.Count,
            ingredients = results
        };
    }
}
