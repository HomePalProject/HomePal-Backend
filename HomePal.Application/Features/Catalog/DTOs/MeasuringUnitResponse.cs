namespace HomePal.Application.Features.Catalog.DTOs;

public class MeasuringUnitResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public DateTime CreatedAt { get; set; }
}
