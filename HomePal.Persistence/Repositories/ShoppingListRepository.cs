using HomePal.Application.Features.ShoppingList.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class ShoppingListRepository : Repository<Domain.Entities.ShoppingList>, IShoppingListRepository
{
    public ShoppingListRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Domain.Entities.ShoppingList?> GetByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Items)
                .ThenInclude(i => i.MeasuringUnit)
            .Include(s => s.Items)
                .ThenInclude(i => i.Category)
            .Include(s => s.Items)
                .ThenInclude(i => i.Offer)
                    .ThenInclude(o => o!.Supermarket)
            .FirstOrDefaultAsync(s => s.HouseholdId == householdId, cancellationToken);
    }

    public async Task<Domain.Entities.ShoppingList> GetOrCreateByHouseholdIdAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        var shoppingList = await GetByHouseholdIdAsync(householdId, cancellationToken);
        if (shoppingList == null)
        {
            shoppingList = new Domain.Entities.ShoppingList
            {
                Id = Guid.NewGuid(),
                HouseholdId = householdId,
                CreatedAt = DateTime.UtcNow
            };
            await _dbSet.AddAsync(shoppingList, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return shoppingList;
    }
}
