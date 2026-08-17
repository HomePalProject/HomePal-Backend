using HomePal.Application.Features.Subscriptions.DTOs;

namespace HomePal.Application.Features.Subscriptions.Interfaces;

public interface IPaymobService
{
    Task<string> GetAuthTokenAsync(CancellationToken cancellationToken = default);

    Task<long> CreateOrderAsync(string authToken, decimal amount, string currency, string merchantOrderId, CancellationToken cancellationToken = default);

    Task<string> GeneratePaymentKeyAsync(
        string authToken,
        long paymobOrderId,
        decimal amount,
        string currency,
        string userEmail,
        string userFirstName,
        string userLastName,
        string? userPhone,
        CancellationToken cancellationToken = default);

    string GetIframeUrl(string paymentToken);

    bool VerifyHmac(PaymobTransactionObject transaction, string receivedHmac);
}
