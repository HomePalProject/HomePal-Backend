using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.Auth.DTOs;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\^$*.[\]{}()?""!@#%&/\\,><':;|_~`]).{8,}$", ErrorMessage = ErrorMessages.Validation.PasswordFormat)]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [Compare(nameof(NewPassword), ErrorMessage = ErrorMessages.Validation.Compare)]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
