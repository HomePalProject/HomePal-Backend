using HomePal.Domain.Common;
using HomePal.Domain.Enums;

namespace HomePal.Domain.Entities;

public class UserSubscription : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public Guid? PlanId { get; set; }
    public SubscriptionPlan? Plan { get; set; }

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Inactive;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow;
    public bool AutoRenew { get; set; } = false;

    public bool IsActiveSubscription => Status == SubscriptionStatus.Active && EndDate >= DateTime.UtcNow;

    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
