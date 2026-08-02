using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.RecipeManagement.Mappers;

public static class RecipeMapper
{
    public static RecipeResponse ToResponse(this Recipe recipe)
    {
        return new RecipeResponse
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Steps = recipe.Steps,
            ServingNum = recipe.ServingNum,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Difficulty = recipe.Difficulty,
            CaloriesPerServing = recipe.CaloriesPerServing,
            FatsPerServing = recipe.FatsPerServing,
            CarbsPerServing = recipe.CarbsPerServing,
            ProteinPerServing = recipe.ProteinPerServing,
            ImageUrl = recipe.ImageUrl,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt,
            Ingredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientDto
            {
                IngredientId = ri.IngredientId,
                IngredientName = ri.Ingredient?.Name ?? new(),
                IngredientPictureUrl = ri.Ingredient?.PictureUrl,
                Amount = ri.Amount,
                MeasurementUnitId = ri.MeasurementUnitId,
                MeasurementUnitName = ri.MeasurementUnit?.Name ?? new(),
                MeasurementUnitSymbol = ri.MeasurementUnit?.Symbol,
                Notes = ri.Notes
            }).ToList(),
            Preferences = recipe.Preferences.Select(p => new RecipePreferenceDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList()
        };
    }
}
