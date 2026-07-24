using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.DTOs;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [EmailAddress(ErrorMessage = ErrorMessages.Validation.Email)]
    public string Email { get; set; } = string.Empty;
}
