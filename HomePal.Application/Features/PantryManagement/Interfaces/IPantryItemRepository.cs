using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.PantryManagement.Interfaces;

public interface IPantryItemRepository : IRepository<PantryItem>
{
    Task<IReadOnlyList<PantryItem>> GetByPantryIdAsync(Guid pantryId, CancellationToken cancellationToken = default);
    Task<PantryItem?> GetByIdAndPantryIdAsync(Guid itemId, Guid pantryId, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<PantryItem> items, CancellationToken cancellationToken = default);
    void RemoveRange(IEnumerable<PantryItem> items);
}
