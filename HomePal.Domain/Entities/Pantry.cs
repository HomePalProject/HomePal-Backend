namespace HomePal.Domain.Entities;

public class Pantry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HouseholdId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Household Household { get; set; } = null!;
    public ICollection<PantryItem> Items { get; set; } = new List<PantryItem>();
}
