using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class RecipeIngredient
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public double Amount { get; set; }

    public Guid MeasurementUnitId { get; set; }
    public MeasurementUnit MeasurementUnit { get; set; } = null!;

    public List<LocalizedItem>? Notes { get; set; }
}
