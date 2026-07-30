using HomePal.Domain.Enums.RecipeEnums;

namespace HomePal.Application.Features.RecipeManagament.DTOs;

public class RecipeResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DifficultyLevel Difficulty { get; set; }

    public TimeSpan TimeToMake { get; set; }

    public int Servings { get; set; }

    public string? ImageUrl { get; set; }

    public List<IngredientResponse> Ingredients { get; set; } = [];

    public List<RecipeStepResponse> Steps { get; set; } = [];
}