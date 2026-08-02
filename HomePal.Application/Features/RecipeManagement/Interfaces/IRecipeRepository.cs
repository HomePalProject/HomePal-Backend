using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.RecipeManagement.Interfaces;

public interface IRecipeRepository : IRepository<Recipe>
{
    Task<PaginatedList<Recipe>> GetFilteredPaginatedAsync(RecipeFilterParams filter, CancellationToken cancellationToken = default);
    Task<Recipe?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
