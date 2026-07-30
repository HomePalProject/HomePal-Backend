namespace HomePal.Application.Features.RecipeManagament.DTOs;

public class RecipeSummaryResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public TimeSpan TimeToMake { get; set; }

    public int Servings { get; set; }
}