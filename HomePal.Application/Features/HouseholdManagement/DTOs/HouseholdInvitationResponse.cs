namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class HouseholdInvitationResponse
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public string HouseholdName { get; set; } = string.Empty;
    public string? InvitedEmail { get; set; }
    public string? InvitedUserName { get; set; }
    public Guid InvitedById { get; set; }
    public string InvitedByName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
