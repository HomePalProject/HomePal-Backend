using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.Options;

public class AdminOptions
{
    public const string SectionName = "AdminOptions";

    [Required(ErrorMessage = "AdminOptions:Email is required.")]
    [EmailAddress(ErrorMessage = "AdminOptions:Email must be a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "AdminOptions:Username is required.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "AdminOptions:Password is required.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "AdminOptions:FullName is required.")]
    public string FullName { get; set; } = string.Empty;
}
