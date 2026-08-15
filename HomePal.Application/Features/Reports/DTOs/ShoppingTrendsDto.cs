using System.Text.Json.Serialization;

namespace HomePal.Application.Features.Reports.DTOs;

public class ShoppingTrendsDto
{
    public CategoryShareDto MostBoughtCategory { get; set; } = new();
    public ItemShareDto MostCommonInventoryItem { get; set; } = new();
    public string MostSuccessfulSupermarket { get; set; } = string.Empty;
    public List<AllergyRankingDto> AllergyRanking { get; set; } = new();
}

public class ItemShareDto
{
    public string Name { get; set; } = string.Empty;
    public double Percentage { get; set; }
}

public class AllergyRankingDto
{
    public string Allergen { get; set; } = string.Empty;
    public double Percentage { get; set; }
}
