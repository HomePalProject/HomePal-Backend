using HomePal.Application.Features.ShoppingList.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class ShoppingListItemRepository : Repository<ShoppingListItem>, IShoppingListItemRepository
{
    public ShoppingListItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<ShoppingListItem>> GetByShoppingListIdAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(s => s.MeasuringUnit)
            .Include(s => s.Category)
            .Include(s => s.Offer)
                .ThenInclude(o => o!.Supermarket)
            .Where(s => s.ShoppingListId == shoppingListId)
            .OrderBy(s => s.IsPurchased)
            .ThenByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingListItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.MeasuringUnit)
            .Include(s => s.Category)
            .Include(s => s.Offer)
                .ThenInclude(o => o!.Supermarket)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task ClearPurchasedAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        var purchasedItems = await _dbSet
            .Where(s => s.ShoppingListId == shoppingListId && s.IsPurchased)
            .ToListAsync(cancellationToken);

        if (purchasedItems.Count > 0)
        {
            _dbSet.RemoveRange(purchasedItems);
        }
    }
}
