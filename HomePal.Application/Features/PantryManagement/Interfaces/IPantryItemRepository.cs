using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.PantryManagement.Interfaces;

public interface IPantryItemRepository : IRepository<PantryItem>
{
    Task<IReadOnlyList<PantryItem>> GetByPantryIdAsync(Guid pantryId, CancellationToken cancellationToken = default);
    Task<PantryItem?> GetByIdAndPantryIdAsync(Guid id, Guid pantryId, CancellationToken cancellationToken = default);
}
