using System.Text.Json.Serialization;

namespace HomePal.Application.Features.Reports.DTOs;

public class GeographicDemographicsDto
{
    public List<DistrictDemographicDto> Districts { get; set; } = new();
    public BudgetMetricDto Budget { get; set; } = new();
    public string AvgBudgetPerHousehold { get; set; } = string.Empty;
    public List<CategoryShareDto> TopCategories { get; set; } = new();
    public List<HouseholdSizeDistributionDto> HouseholdSize { get; set; } = new();
    public List<GenderSplitDto> GenderSplit { get; set; } = new();
    public List<AgeSplitDto> AgeSplit { get; set; } = new();
}

public class DistrictDemographicDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Growth { get; set; } = string.Empty;
    public string HhDensity { get; set; } = string.Empty;
    public string AvgIncome { get; set; } = string.Empty;
    public string Pop { get; set; } = string.Empty;
    public double Intensity { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public int Radius { get; set; }
}

public class BudgetMetricDto
{
    public string Value { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}

public class HouseholdSizeDistributionDto
{
    public string Size { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class GenderSplitDto
{
    public string Gender { get; set; } = string.Empty;
    public double Percentage { get; set; }
}

public class AgeSplitDto
{
    public string Range { get; set; } = string.Empty;
    public double Percentage { get; set; }
}
