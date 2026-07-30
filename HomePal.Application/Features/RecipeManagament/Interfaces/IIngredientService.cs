using HomePal.Application.Features.RecipeManagament.DTOs;
using HomePal.Shared.Results;

public interface IIngredientService
{
    Task<Result<IReadOnlyList<IngredientResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<IngredientResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<IngredientResponse>> CreateAsync(
        CreateIngredientRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
        Guid id,
        UpdateIngredientRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}