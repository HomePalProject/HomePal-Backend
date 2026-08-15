using System.Text.Json.Serialization;

namespace HomePal.Application.Features.Reports.DTOs;

public class MealPlansSummaryDto
{
    public double MealPlansPerHousehold { get; set; }
    public int MealPlansTotal { get; set; }
}
