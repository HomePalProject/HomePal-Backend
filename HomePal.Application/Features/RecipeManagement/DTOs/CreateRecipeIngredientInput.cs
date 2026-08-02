using HomePal.Domain.Common;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class CreateRecipeIngredientInput
{
    public Guid IngredientId { get; set; }
    public double Amount { get; set; }
    public Guid MeasurementUnitId { get; set; }
    public List<LocalizedItem>? Notes { get; set; }
}
