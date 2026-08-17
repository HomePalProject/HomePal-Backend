namespace HomePal.Application.Features.Subscriptions.DTOs;

public class SubscriptionPlanResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EGP";
    public int DurationInDays { get; set; }
}
