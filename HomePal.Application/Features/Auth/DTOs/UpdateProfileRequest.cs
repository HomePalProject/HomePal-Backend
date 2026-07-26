using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Auth.DTOs;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [EnumDataType(typeof(Gender), ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public Gender? Gender { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public DateOnly? BirthDate { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string Governorate { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string City { get; set; } = string.Empty;
}
