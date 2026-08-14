using HomePal.Domain.Common;
using HomePal.Domain.Enums;

namespace HomePal.Domain.Entities;

public class HouseholdInvitation : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HouseholdId { get; set; }
    public string? InvitedEmail { get; set; }
    public string? InvitedUserName { get; set; }
    public Guid InvitedById { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString("N");
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

    public Household Household { get; set; } = null!;
    public ApplicationUser InvitedBy { get; set; } = null!;
}
