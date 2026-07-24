using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.DTOs;

public class GoogleLoginRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string IdToken { get; set; } = string.Empty;
}
