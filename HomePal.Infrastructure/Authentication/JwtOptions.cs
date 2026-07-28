using System.ComponentModel.DataAnnotations;

namespace HomePal.Infrastructure.Authentication;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";

    [Required(ErrorMessage = "JwtOptions:Issuer is required.")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "JwtOptions:Audience is required.")]
    public string Audience { get; set; } = string.Empty;

    [Required(ErrorMessage = "JwtOptions:SecretKey is required.")]
    [MinLength(32, ErrorMessage = "JwtOptions:SecretKey must be at least 32 characters long.")]
    public string SecretKey { get; set; } = string.Empty;

    [Range(1, 10080, ErrorMessage = "JwtOptions:AccessTokenExpirationMinutes must be between 1 minute and 7 days.")]
    public int AccessTokenExpirationMinutes { get; set; }

    [Range(1, 365, ErrorMessage = "JwtOptions:RefreshTokenExpirationDays must be between 1 and 365 days.")]
    public int RefreshTokenExpirationDays { get; set; }
}
