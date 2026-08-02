using HomePal.Domain.Common;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class CreateMeasurementUnitRequest
{
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Symbol { get; set; }
    public List<LocalizedItem>? Description { get; set; }
}
