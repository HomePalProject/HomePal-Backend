namespace HomePal.Application.Features.MealPlanning.Interfaces;

public interface IFoodRecipeSearchService
{
    Task<IReadOnlyList<string>> SearchAsync(
        string query,
        int limit = 5,
        CancellationToken cancellationToken = default);
}
