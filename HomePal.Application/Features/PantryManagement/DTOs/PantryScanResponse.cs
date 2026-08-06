namespace HomePal.Application.Features.PantryManagement.DTOs;

public class PantryScanItemDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public Guid MeasuringUnitId { get; set; }
    public string? MeasuringUnitName { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime? SuggestedExpireDate { get; set; }
}

public class PantryScanResponse
{
    public List<PantryScanItemDto> Items { get; set; } = new();
}
