using HomePal.Application.Features.RecipeManagament.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagament.Interfaces;

public interface IRecipeService
{
    Task<Result<IReadOnlyList<RecipeSummaryResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<RecipeResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<RecipeResponse>> CreateAsync(
        CreateRecipeRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        Guid id,
        UpdateRecipeRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}