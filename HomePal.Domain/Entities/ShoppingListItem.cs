using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class ShoppingListItem : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ShoppingListId { get; set; }
    public ShoppingList ShoppingList { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public double Quantity { get; set; } = 1;
    public int PortionCount { get; set; } = 1;
    public decimal? Price { get; set; }

    public Guid? MeasuringUnitId { get; set; }
    public MeasuringUnit? MeasuringUnit { get; set; }

    public Guid? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }

    public Guid? OfferId { get; set; }
    public Offer? Offer { get; set; }

    public Guid? MealPlanId { get; set; }
    public MealPlan? MealPlan { get; set; }

    public bool IsPurchased { get; set; } = false;
    public string? Notes { get; set; }
}
