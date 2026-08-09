using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.ShoppingList.Interfaces;

public interface IShoppingListItemRepository : IRepository<ShoppingListItem>
{
    Task<List<ShoppingListItem>> GetByShoppingListIdAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
    Task<ShoppingListItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task ClearPurchasedAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
}
