using HomePal.Application.Features.RecipeManagament.DTOs;
using HomePal.Application.Features.RecipeManagament.Mappers;

using HomePal.Domain.Entities.Recipe;

namespace HomePal.Application.Features.RecipeManagament.Mappers;

public static class RecipeMapper
{
    public static RecipeResponse ToResponse(this Recipe recipe)
    {
        return new RecipeResponse
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Difficulty = recipe.Difficulty,
            TimeToMake = recipe.TimeToMake,
            Servings = recipe.Servings,
            ImageUrl = recipe.ImageUrl,

            Ingredients = recipe.Ingredients
                .Select(i => i.ToResponse())
                .ToList(),

            Steps = recipe.Steps
                .OrderBy(s => s.StepOrder)
                .Select(s => s.ToResponse())
                .ToList()
        };
    }

    public static RecipeSummaryResponse ToSummaryResponse(this Recipe recipe)
    {
        return new RecipeSummaryResponse
        {
            Id = recipe.Id,
            Name = recipe.Name,
            ImageUrl = recipe.ImageUrl,
            TimeToMake = recipe.TimeToMake,
            Servings = recipe.Servings
        };
    }

    public static IReadOnlyList<RecipeSummaryResponse> ToSummaryResponse(
        this IEnumerable<Recipe> recipes)
    {
        return recipes
            .Select(r => r.ToSummaryResponse())
            .ToList();
    }
}