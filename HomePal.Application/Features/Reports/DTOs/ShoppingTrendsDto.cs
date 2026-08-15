using System.Text.Json.Serialization;

namespace HomePal.Application.Features.Reports.DTOs;

public class ShoppingTrendsDto
{
    public CategoryShareDto MostBoughtCategory { get; set; } = new();
    public CategoryShareDto MostCommonInventoryCategory { get; set; } = new();
    public string MostSuccessfulSupermarket { get; set; } = string.Empty;
    public List<PreferenceRankingDto> PreferenceRanking { get; set; } = new();
}

public class PreferenceRankingDto
{
    public string Preference { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public double Percentage { get; set; }
}
