using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string EmailOrUsername { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string Password { get; set; } = string.Empty;
}
