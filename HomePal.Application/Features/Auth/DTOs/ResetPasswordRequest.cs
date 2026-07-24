using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.DTOs;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [EmailAddress(ErrorMessage = ErrorMessages.Validation.Email)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\^$*.[\]{}()?""!@#%&/\\,><':;|_~`]).{8,}$", ErrorMessage = ErrorMessages.Validation.PasswordFormat)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [Compare(nameof(NewPassword), ErrorMessage = ErrorMessages.Validation.Compare)]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
