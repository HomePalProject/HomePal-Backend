using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.ShoppingList.Interfaces;

public interface IShoppingListRepository : IRepository<Domain.Entities.ShoppingList>
{
    Task<Domain.Entities.ShoppingList?> GetByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default);
    Task<Domain.Entities.ShoppingList> GetOrCreateByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default);
}
