using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class ShoppingList : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;

    public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}
