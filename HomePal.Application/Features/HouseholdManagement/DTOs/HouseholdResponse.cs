namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class HouseholdResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public Guid? GovernorateId { get; set; }
    public string? Governorate { get; set; }
    public Guid? CityId { get; set; }
    public string? City { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MembersCount { get; set; }
}
