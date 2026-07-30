namespace HomePal.Application.Features.PantryManagement.DTOs;

public class PantryItemResponse
{
    public Guid Id { get; set; }
    public Guid PantryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? ExpireDate { get; set; }
    public decimal Quantity { get; set; }
    public string MeasuringUnit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
