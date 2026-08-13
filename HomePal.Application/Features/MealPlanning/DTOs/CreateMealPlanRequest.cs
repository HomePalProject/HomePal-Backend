using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.MealPlanning.DTOs;

public class CreateMealPlanRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(0, (double)decimal.MaxValue)]
    public decimal TotalEstimatedCost { get; set; }

    [Required]
    public string PlanData { get; set; } = string.Empty;
}
