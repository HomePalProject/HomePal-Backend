using HomePal.Domain.Enums.RecipeEnums;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.RecipeManagament.DTOs;

public class CreateRecipeRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DifficultyLevel Difficulty { get; set; }

    [Required]
    public TimeSpan TimeToMake { get; set; }

    [Range(1, 100)]
    public int Servings { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    [Required]
    [MinLength(1)]
    public List<RecipeIngredientRequest> Ingredients { get; set; } = [];

    [Required]
    [MinLength(1)]
    public List<RecipeStepRequest> Steps { get; set; } = [];
}