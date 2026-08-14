using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class MealPlan : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalEstimatedCost { get; set; }
    public string PlanData { get; set; } = string.Empty;
}
