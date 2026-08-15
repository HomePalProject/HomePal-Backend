using System.Text.Json.Serialization;

namespace HomePal.Application.Features.Reports.DTOs;

public class HouseholdsSummaryDto
{
    public int TotalHouseholds { get; set; }
    public int ActiveHouseholds { get; set; }
    public double AvgHouseholdSize { get; set; }
    public string AvgHouseholdIncome { get; set; } = string.Empty;
    public string GrowthRate { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public List<RegionHouseholdSummaryDto> TopRegions { get; set; } = new();
    public List<HouseholdSizeCountDto> SizeDistribution { get; set; } = new();
}

public class RegionHouseholdSummaryDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class HouseholdSizeCountDto
{
    public string Size { get; set; } = string.Empty;
    public int Count { get; set; }
}
