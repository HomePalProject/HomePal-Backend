using HomePal.Domain.Enums;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class RecipeFilterParams : PaginationRequest
{
    public string? Search { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public int? MaxPrepTime { get; set; }
    public int? MaxCookTime { get; set; }
    public double? MaxCalories { get; set; }
    public List<Guid>? PreferenceIds { get; set; }
    public List<Guid>? IngredientIds { get; set; }
}
