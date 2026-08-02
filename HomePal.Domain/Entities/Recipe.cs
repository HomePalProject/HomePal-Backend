using HomePal.Domain.Common;
using HomePal.Domain.Enums;

namespace HomePal.Domain.Entities;

public class Recipe
{
    public Guid Id { get; set; } = Guid.NewGuid();
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
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<Preference> Preferences { get; set; } = new List<Preference>();
}
