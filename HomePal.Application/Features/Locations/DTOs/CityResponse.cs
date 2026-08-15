namespace HomePal.Application.Features.Locations.DTOs;

public class CityResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid GovernorateId { get; set; }
    public string GovernorateCode { get; set; } = string.Empty;
    public string? GovernorateName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
