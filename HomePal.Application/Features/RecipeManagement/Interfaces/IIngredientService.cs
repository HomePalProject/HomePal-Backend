using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagement.Interfaces;

public interface IIngredientService
{
    Task<Result<PaginatedList<IngredientResponse>>> GetIngredientsAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default);
    Task<Result<IngredientResponse>> GetIngredientByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IngredientResponse>> CreateIngredientAsync(CreateIngredientRequest request, CancellationToken cancellationToken = default);
    Task<Result<IngredientResponse>> UpdateIngredientAsync(Guid id, UpdateIngredientRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteIngredientAsync(Guid id, CancellationToken cancellationToken = default);
}
