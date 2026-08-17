using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HomePal.Application.Features.Subscriptions.DTOs;
using HomePal.Application.Features.Subscriptions.Interfaces;
using HomePal.Application.Features.Subscriptions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HomePal.Infrastructure.Payment;

public class PaymobService : IPaymobService
{
    private readonly HttpClient _httpClient;
    private readonly PaymobOptions _options;
    private readonly ILogger<PaymobService> _logger;

    public PaymobService(
        HttpClient httpClient,
        IOptions<PaymobOptions> options,
        ILogger<PaymobService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetAuthTokenAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/api/auth/tokens";
        var payload = new { api_key = _options.ApiKey };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content, cancellationToken);

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("token", out var tokenElement))
        {
            return tokenElement.GetString() ?? throw new InvalidOperationException("Paymob token was empty.");
        }

        throw new InvalidOperationException("Failed to extract token from Paymob auth response.");
    }

    public async Task<long> CreateOrderAsync(
        string authToken,
        decimal amount,
        string currency,
        string merchantOrderId,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/api/ecommerce/orders";
        var amountCents = (long)Math.Round(amount * 100, 0);

        var payload = new
        {
            auth_token = authToken,
            delivery_needed = "false",
            amount_cents = amountCents.ToString(),
            currency = currency,
            merchant_order_id = merchantOrderId,
            items = Array.Empty<object>()
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content, cancellationToken);

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("id", out var idElement))
        {
            return idElement.GetInt64();
        }

        throw new InvalidOperationException("Failed to extract order id from Paymob order creation response.");
    }

    public async Task<string> GeneratePaymentKeyAsync(
        string authToken,
        long paymobOrderId,
        decimal amount,
        string currency,
        string userEmail,
        string userFirstName,
        string userLastName,
        string? userPhone,
        CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/api/acceptance/payment_keys";
        var amountCents = (long)Math.Round(amount * 100, 0);

        var integrationId = int.TryParse(_options.IntegrationId, out var id) ? id : 0;

        var payload = new
        {
            auth_token = authToken,
            amount_cents = amountCents.ToString(),
            expiration = 3600,
            order_id = paymobOrderId.ToString(),
            billing_data = new
            {
                apartment = "NA",
                email = string.IsNullOrWhiteSpace(userEmail) ? "customer@homepal.app" : userEmail,
                floor = "NA",
                first_name = string.IsNullOrWhiteSpace(userFirstName) ? "HomePal" : userFirstName,
                street = "NA",
                building = "NA",
                phone_number = string.IsNullOrWhiteSpace(userPhone) ? "+201000000000" : userPhone,
                shipping_method = "NA",
                postal_code = "NA",
                city = "Cairo",
                country = "EG",
                last_name = string.IsNullOrWhiteSpace(userLastName) ? "User" : userLastName,
                state = "Cairo"
            },
            currency = currency,
            integration_id = integrationId,
            lock_order_when_paid = "false"
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content, cancellationToken);

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("token", out var tokenElement))
        {
            return tokenElement.GetString() ?? throw new InvalidOperationException("Paymob payment key token was empty.");
        }

        throw new InvalidOperationException("Failed to extract payment key token from Paymob response.");
    }

    public string GetIframeUrl(string paymentToken)
    {
        return $"https://accept.paymob.com/api/acceptance/iframes/{_options.IframeId}?payment_token={paymentToken}";
    }

    public bool VerifyHmac(PaymobTransactionObject transaction, string receivedHmac)
    {
        if (string.IsNullOrWhiteSpace(_options.HmacSecret) || string.IsNullOrWhiteSpace(receivedHmac))
        {
            return false;
        }

        // Paymob standard HMAC concatenation order
        var sb = new StringBuilder();
        sb.Append(transaction.AmountCents);
        sb.Append(transaction.CreatedAt);
        sb.Append(transaction.Currency);
        sb.Append(transaction.ErrorOccured.ToString().ToLowerInvariant());
        sb.Append(transaction.HasParentTransaction.ToString().ToLowerInvariant());
        sb.Append(transaction.Id);
        sb.Append(transaction.IntegrationId);
        sb.Append(transaction.Is3dSecure.ToString().ToLowerInvariant());
        sb.Append(transaction.IsAuth.ToString().ToLowerInvariant());
        sb.Append(transaction.IsCapture.ToString().ToLowerInvariant());
        sb.Append(transaction.IsRefunded.ToString().ToLowerInvariant());
        sb.Append(transaction.IsStandalonePayment.ToString().ToLowerInvariant());
        sb.Append(transaction.IsVoided.ToString().ToLowerInvariant());
        sb.Append(transaction.Order?.Id.ToString() ?? string.Empty);
        sb.Append(transaction.Owner);
        sb.Append(transaction.Pending.ToString().ToLowerInvariant());
        sb.Append(transaction.SourceData?.Pan ?? string.Empty);
        sb.Append(transaction.SourceData?.SubType ?? string.Empty);
        sb.Append(transaction.SourceData?.Type ?? string.Empty);
        sb.Append(transaction.Success.ToString().ToLowerInvariant());

        var concatenated = sb.ToString();

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_options.HmacSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
        var calculatedHmac = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return string.Equals(calculatedHmac, receivedHmac.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
    }
}
