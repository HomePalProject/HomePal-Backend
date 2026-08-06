using HomePal.Domain.Common;

namespace HomePal.Application.Features.PantryManagement.DTOs;

public class PantryItemResponse
{
    public Guid Id { get; set; }
    public Guid PantryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? ExpireDate { get; set; }
    public decimal Quantity { get; set; }

    public Guid MeasuringUnitId { get; set; }
    public List<LocalizedItem>? MeasuringUnitName { get; set; }
    public List<LocalizedItem>? MeasuringUnitSymbol { get; set; }

    public Guid CategoryId { get; set; }
    public List<LocalizedItem>? CategoryName { get; set; }
    public string? CategoryImagePath { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
