using HomePal.Domain.Common;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class MeasurementUnitResponse
{
    public Guid Id { get; set; }
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Symbol { get; set; }
    public List<LocalizedItem>? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
