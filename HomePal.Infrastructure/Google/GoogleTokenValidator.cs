using Google.Apis.Auth;
using HomePal.Application.Features.Auth.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HomePal.Infrastructure.Google;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options;
    private readonly ILogger<GoogleTokenValidator> _logger;

    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options, ILogger<GoogleTokenValidator> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<GooglePayload?> ValidateTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings();

            var audiences = new List<string>();
            if (!string.IsNullOrWhiteSpace(_options.ClientId))
            {
                audiences.Add(_options.ClientId);
            }
            if (!string.IsNullOrWhiteSpace(_options.AndroidClientId))
            {
                audiences.Add(_options.AndroidClientId);
            }

            if (audiences.Count > 0)
            {
                settings.Audience = audiences;
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            if (payload == null)
            {
                return null;
            }

            return new GooglePayload
            {
                Subject = payload.Subject,
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                Name = payload.Name,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate Google ID Token.");
            return null;
        }
    }
}
