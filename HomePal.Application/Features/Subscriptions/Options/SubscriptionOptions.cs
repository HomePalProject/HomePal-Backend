namespace HomePal.Application.Features.Subscriptions.Options;

public class SubscriptionOptions
{
    public const string SectionName = "SubscriptionOptions";

    /// <summary>
    /// Indicates whether active subscription enforcement is enabled.
    /// When false, subscription checks (e.g. [RequireActiveSubscription]) are bypassed.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
