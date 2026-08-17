using HomePal.Domain.Enums;

namespace HomePal.Application.Features.Subscriptions.DTOs;

public class UserSubscriptionResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? PlanId { get; set; }
    public string? PlanName { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int RemainingDays { get; set; }
}
