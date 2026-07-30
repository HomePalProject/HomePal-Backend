using HomePal.Application.Features.RecipeManagament.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagament.Interfaces;

public interface IIngredientService
{
    Task<Result<IReadOnlyList<IngredientResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Result<IngredientResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}