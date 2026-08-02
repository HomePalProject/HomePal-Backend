using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagement.Interfaces;

public interface IRecipeService
{
    Task<Result<PaginatedList<RecipeResponse>>> GetRecipesAsync(RecipeFilterParams filter, CancellationToken cancellationToken = default);
    Task<Result<RecipeResponse>> GetRecipeByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<RecipeResponse>> CreateRecipeAsync(CreateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<Result<RecipeResponse>> UpdateRecipeAsync(Guid id, UpdateRecipeRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteRecipeAsync(Guid id, CancellationToken cancellationToken = default);
}
