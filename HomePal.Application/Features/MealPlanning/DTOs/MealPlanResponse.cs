namespace HomePal.Application.Features.MealPlanning.DTOs;

public class MealPlanResponse
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalEstimatedCost { get; set; }
    public string PlanData { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
