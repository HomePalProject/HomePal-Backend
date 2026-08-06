namespace HomePal.Domain.Entities;

public class PantryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid PantryId { get; set; }
    public Pantry Pantry { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public DateTime? ExpireDate { get; set; }
    public decimal Quantity { get; set; }

    public Guid MeasuringUnitId { get; set; }
    public MeasuringUnit MeasuringUnit { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public ProductCategory Category { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
