using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.RecipeManagement.Mappers;

public static class IngredientMapper
{
    public static IngredientResponse ToResponse(this Ingredient ingredient)
    {
        return new IngredientResponse
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Description = ingredient.Description,
            PictureUrl = ingredient.PictureUrl,
            CreatedAt = ingredient.CreatedAt,
            UpdatedAt = ingredient.UpdatedAt
        };
    }
}
