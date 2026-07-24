using HomePal.Shared.Results;
using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Enums;

namespace HomePal.Application.Features.Auth.DTOs;

public class CompleteGoogleProfileRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [EnumDataType(typeof(Gender), ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public DateOnly BirthDate { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string Governorate { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string City { get; set; } = string.Empty;
}
