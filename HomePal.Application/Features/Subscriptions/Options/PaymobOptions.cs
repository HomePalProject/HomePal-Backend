using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Subscriptions.Options;

public class PaymobOptions
{
    public const string SectionName = "PaymobOptions";

    public string ApiKey { get; set; } = string.Empty;

    public string HmacSecret { get; set; } = string.Empty;

    public string IntegrationId { get; set; } = string.Empty;

    public string IframeId { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://accept.paymob.com";

    public string Currency { get; set; } = "EGP";

    public string? CallbackUrl { get; set; }
}
