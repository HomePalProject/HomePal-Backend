using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Enums;

namespace HomePal.Application.Features.Auth.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [EnumDataType(typeof(Gender), ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public Gender? Gender { get; set; }
 
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(50, MinimumLength = 3, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [EmailAddress(ErrorMessage = ErrorMessages.Validation.Email)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\^$*.[\]{}()?""!@#%&/\\,><':;|_~`]).{8,}$", ErrorMessage = ErrorMessages.Validation.PasswordFormat)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [Compare(nameof(Password), ErrorMessage = ErrorMessages.Validation.Compare)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public DateOnly? BirthDate { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string Governorate { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string City { get; set; } = string.Empty;
}
