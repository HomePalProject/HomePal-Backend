using HomePal.Domain.Common;
using HomePal.Domain.Enums;

namespace HomePal.Domain.Entities;

public class PaymentTransaction : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid? SubscriptionId { get; set; }
    public UserSubscription? Subscription { get; set; }

    public Guid? PlanId { get; set; }
    public SubscriptionPlan? Plan { get; set; }

    public string? PaymobOrderId { get; set; }
    public string? PaymobTransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; set; }
    public string? RawCallbackData { get; set; }
}
