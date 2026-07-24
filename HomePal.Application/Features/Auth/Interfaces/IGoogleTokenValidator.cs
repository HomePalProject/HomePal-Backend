namespace HomePal.Application.Features.Auth.Interfaces;

public class GooglePayload
{
    public string Subject { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
}

public interface IGoogleTokenValidator
{
    Task<GooglePayload?> ValidateTokenAsync(string idToken, CancellationToken cancellationToken = default);
}
