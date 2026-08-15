using System.Text.Json.Serialization;

namespace HomePal.Application.Features.Reports.DTOs;

public class AnalyticsOverviewDto
{
    public List<TopSupermarketChainDto> TopSupermarketChains { get; set; } = new();
    public List<UserDistributionDto> UserDistribution { get; set; } = new();
    public List<CategoryShareDto> TopCategories { get; set; } = new();
}

public class TopSupermarketChainDto
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class UserDistributionDto
{
    public string Region { get; set; } = string.Empty;
    public int Households { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class CategoryShareDto
{
    public string Name { get; set; } = string.Empty;
    public double Percentage { get; set; }
}
