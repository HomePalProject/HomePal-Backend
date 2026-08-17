using HomePal.Domain.Enums;

namespace HomePal.Application.Features.Subscriptions.DTOs;

public class PaymentTransactionResponse
{
    public Guid Id { get; set; }
    public string? PaymobOrderId { get; set; }
    public string? PaymobTransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public PaymentStatus Status { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
}
