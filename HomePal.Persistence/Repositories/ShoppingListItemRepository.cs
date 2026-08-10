using HomePal.Application.Features.ShoppingList.Interfaces;
using HomePal.Domain.Common;
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

    public async Task UpdateFromOfferAsync(Offer offer, CancellationToken cancellationToken = default)
    {
        var items = await _dbSet
            .Where(s => s.OfferId == offer.Id)
            .ToListAsync(cancellationToken);

        if (items.Count > 0)
        {
            var offerNameEn = offer.Name.Get("en");
            var price = offer.DiscountedPrice > 0 ? offer.DiscountedPrice : offer.OriginalPrice;

            foreach (var item in items)
            {
                item.Name = !string.IsNullOrWhiteSpace(offerNameEn) ? offerNameEn : item.Name;
                item.Quantity = offer.Quantity > 0 ? offer.Quantity : item.Quantity;
                item.Price = price;
                item.MeasuringUnitId = offer.UnitId;
                item.CategoryId = offer.CategoryId;
                item.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
