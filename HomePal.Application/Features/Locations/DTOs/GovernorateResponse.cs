namespace HomePal.Application.Features.Locations.DTOs;

public class GovernorateResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int CitiesCount { get; set; }
}
