using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.DTOs;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string RefreshToken { get; set; } = string.Empty;
}
