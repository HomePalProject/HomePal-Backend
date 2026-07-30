using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities.Recipe;

namespace HomePal.Application.Features.RecipeManagament.Interfaces;

public interface IRecipeRepository : IRepository<Recipe>
{
    Task<Recipe?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Recipe?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}