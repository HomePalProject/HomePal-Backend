using HomePal.Domain.Common;
using HomePal.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class CreateRecipeRequest
{
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }
    public List<LocalizedItem> Steps { get; set; } = new();
    public int ServingNum { get; set; }
    public int? PrepTimeMinutes { get; set; }
    public int? CookTimeMinutes { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public double? CaloriesPerServing { get; set; }
    public double? FatsPerServing { get; set; }
    public double? CarbsPerServing { get; set; }
    public double? ProteinPerServing { get; set; }
    public IFormFile? Image { get; set; }

    public List<CreateRecipeIngredientInput> Ingredients { get; set; } = new();
    public List<Guid> PreferenceIds { get; set; } = new();
}
