using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.DTOs;

public class ConfirmEmailRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string Token { get; set; } = string.Empty;
}
