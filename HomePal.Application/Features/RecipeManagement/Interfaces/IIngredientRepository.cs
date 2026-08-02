using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.RecipeManagement.Interfaces;

public interface IIngredientRepository : IRepository<Ingredient>
{
    Task<PaginatedList<Ingredient>> GetPaginatedAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsUsedInRecipesAsync(Guid ingredientId, CancellationToken cancellationToken = default);
}
