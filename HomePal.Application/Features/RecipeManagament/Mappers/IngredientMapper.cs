using HomePal.Application.Features.RecipeManagament.DTOs;
using HomePal.Domain.Entities.Recipe;

namespace HomePal.Application.Features.RecipeManagament.Mappers;

public static class IngredientMapper
{
    public static IngredientResponse ToResponse(this Ingredient ingredient)
    {
        return new IngredientResponse
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Unit = ingredient.DefaultUnit
        };
    }

    public static IngredientResponse ToResponse(this RecipeIngredient recipeIngredient)
    {
        return new IngredientResponse
        {
            Id = recipeIngredient.IngredientId,
            Name = recipeIngredient.Ingredient.Name,
            Amount = recipeIngredient.Amount,
            Unit = recipeIngredient.Unit
        };
    }

    public static RecipeStepResponse ToResponse(this RecipeStep step)
    {
        return new RecipeStepResponse
        {
            Order = step.StepOrder,
            Description = step.Description
        };
    }

    public static IReadOnlyList<IngredientResponse> ToResponse(
        this IEnumerable<Ingredient> ingredients)
    {
        return ingredients
            .Select(i => i.ToResponse())
            .ToList();
    }
}