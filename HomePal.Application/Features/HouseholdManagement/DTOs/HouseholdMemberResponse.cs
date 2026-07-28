using HomePal.Domain.Enums;

namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class HouseholdMemberResponse
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsRegistered => UserId.HasValue;
    public DateTime JoinedAt { get; set; }
}
