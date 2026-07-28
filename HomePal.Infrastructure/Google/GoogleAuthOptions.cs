using System.ComponentModel.DataAnnotations;

namespace HomePal.Infrastructure.Google;

public class GoogleAuthOptions
{
    public const string SectionName = "GoogleAuthOptions";

    [Required(ErrorMessage = "GoogleAuthOptions:ClientId is required.")]
    public string ClientId { get; set; } = string.Empty;

    [Required(ErrorMessage = "GoogleAuthOptions:AndroidClientId is required.")]
    public string AndroidClientId { get; set; } = string.Empty;
}
