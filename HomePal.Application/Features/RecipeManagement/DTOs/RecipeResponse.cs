using HomePal.Domain.Common;
using HomePal.Domain.Enums;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class RecipeResponse
{
    public Guid Id { get; set; }
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }
    public List<LocalizedItem> Steps { get; set; } = new();
    public int ServingNum { get; set; }
    public int? PrepTimeMinutes { get; set; }
    public int? CookTimeMinutes { get; set; }
    public int? TotalTimeMinutes => (PrepTimeMinutes ?? 0) + (CookTimeMinutes ?? 0);
    public DifficultyLevel? Difficulty { get; set; }
    public double? CaloriesPerServing { get; set; }
    public double? FatsPerServing { get; set; }
    public double? CarbsPerServing { get; set; }
    public double? ProteinPerServing { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    public List<RecipePreferenceDto> Preferences { get; set; } = new();
}

public class RecipePreferenceDto
{
    public Guid Id { get; set; }
    public List<LocalizedItem> Name { get; set; } = new();
}
