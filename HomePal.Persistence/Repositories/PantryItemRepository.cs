using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class PantryItemRepository : Repository<PantryItem>, IPantryItemRepository
{
    public PantryItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PantryItem>> GetByPantryIdAsync(Guid pantryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.MeasuringUnit)
            .Include(i => i.Category)
            .Where(i => i.PantryId == pantryId)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PantryItem?> GetByIdAndPantryIdAsync(Guid itemId, Guid pantryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.MeasuringUnit)
            .Include(i => i.Category)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.PantryId == pantryId, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<PantryItem> items, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(items, cancellationToken);
    }

    public void RemoveRange(IEnumerable<PantryItem> items)
    {
        _dbSet.RemoveRange(items);
    }
}
