namespace HomePal.Application.Features.Locations.DTOs;

public class CityQueryRequest
{
    public Guid? GovernorateId { get; set; }
    public string? GovernorateCode { get; set; }
    public string? Query { get; set; }
}
