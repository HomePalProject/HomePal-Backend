using System.ComponentModel;
using HomePal.Application.Features.MealPlanning.Interfaces;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

/// <summary>
/// AI Agent Tool for searching food recipes via MongoDB Atlas Vector Search.
/// </summary>
public class RecipeSearchTools
{
    private readonly IFoodRecipeSearchService _recipeSearchService;

    public RecipeSearchTools(IFoodRecipeSearchService recipeSearchService)
    {
        _recipeSearchService = recipeSearchService;
    }

    [Description("Searches the recipe database using semantic vector search for recipes matching dish names, ingredients, meal types, cuisines, or dietary preferences.")]
    public async Task<object> SearchRecipesAsync(
        [Description("The search query (e.g. 'grilled chicken with vegetables', 'quick high protein breakfast', 'vegetarian pasta').")] string query,
        [Description("The maximum number of recipe results to return (1-10). Default is 5.")] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var clampedLimit = Math.Clamp(limit, 1, 10);
        var results = await _recipeSearchService.SearchAsync(query, clampedLimit, cancellationToken);

        return new
        {
            success = true,
            query,
            totalResults = results.Count,
            recipes = results
        };
    }
}
