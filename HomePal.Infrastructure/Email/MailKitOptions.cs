using System.ComponentModel.DataAnnotations;

namespace HomePal.Infrastructure.Email;

public class MailKitOptions
{
    public const string SectionName = "MailKitOptions";

    [Required(ErrorMessage = "MailKitOptions:Host is required.")]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535, ErrorMessage = "MailKitOptions:Port must be between 1 and 65535.")]
    public int Port { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool EnableSsl { get; set; }

    [Required(ErrorMessage = "MailKitOptions:SenderName is required.")]
    public string SenderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "MailKitOptions:SenderEmail is required.")]
    [EmailAddress(ErrorMessage = "MailKitOptions:SenderEmail must be a valid email address.")]
    public string SenderEmail { get; set; } = string.Empty;
}
