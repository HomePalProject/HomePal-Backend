using System.Text.Json.Serialization;

namespace HomePal.Application.Features.Reports.DTOs;

public class UserDemographicsDto
{
    public double AvgAgeHouseholders { get; set; }
    public double AvgAgeUsers { get; set; }
    public List<GenderSplitDto> GenderSplitHouseholders { get; set; } = new();
    public List<GenderSplitDto> GenderSplitUsers { get; set; } = new();
}
