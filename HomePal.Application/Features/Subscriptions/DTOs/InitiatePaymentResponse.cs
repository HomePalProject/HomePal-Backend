namespace HomePal.Application.Features.Subscriptions.DTOs;

public class InitiatePaymentResponse
{
    public string PaymentToken { get; set; } = string.Empty;
    public string IframeUrl { get; set; } = string.Empty;
    public string PaymobOrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
}
