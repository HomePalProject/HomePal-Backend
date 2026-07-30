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
            .Where(pi => pi.PantryId == pantryId)
            .OrderByDescending(pi => pi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PantryItem?> GetByIdAndPantryIdAsync(Guid id, Guid pantryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pi => pi.Id == id && pi.PantryId == pantryId, cancellationToken);
    }
}
