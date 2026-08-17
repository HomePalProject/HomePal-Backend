using HomePal.Domain.Common;

namespace HomePal.Domain.Entities;

public class SubscriptionPlan : BaseAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EGP";
    public int DurationInDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
