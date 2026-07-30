namespace HomePal.Domain.Entities;

public class PantryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PantryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? ExpireDate { get; set; }
    public decimal Quantity { get; set; }
    public string MeasuringUnit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Pantry Pantry { get; set; } = null!;
}
