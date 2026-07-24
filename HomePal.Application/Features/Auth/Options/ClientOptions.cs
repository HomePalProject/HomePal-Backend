using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.Options;

public class ClientOptions
{
    public const string SectionName = "ClientOptions";

    [Required(ErrorMessage = "ClientOptions:BaseUrl is required.")]
    [Url(ErrorMessage = "ClientOptions:BaseUrl must be a valid URL.")]
    public string BaseUrl { get; set; } = string.Empty;
}
