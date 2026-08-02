using HomePal.Domain.Common;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class RecipeIngredientDto
{
    public Guid IngredientId { get; set; }
    public List<LocalizedItem> IngredientName { get; set; } = new();
    public string? IngredientPictureUrl { get; set; }

    public double Amount { get; set; }

    public Guid MeasurementUnitId { get; set; }
    public List<LocalizedItem> MeasurementUnitName { get; set; } = new();
    public List<LocalizedItem>? MeasurementUnitSymbol { get; set; }

    public List<LocalizedItem>? Notes { get; set; }
}
