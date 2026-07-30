using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities.Recipe;

namespace HomePal.Application.Features.RecipeManagament.Interfaces;

public interface IIngredientRepository : IRepository<Ingredient>
{
    Task<Ingredient?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}