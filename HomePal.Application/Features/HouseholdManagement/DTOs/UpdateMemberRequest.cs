using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Constants;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class UpdateMemberRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [EnumDataType(typeof(Gender), ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public Gender Gender { get; set; } = Gender.Male;

    public DateOnly? DateOfBirth { get; set; }

    [AllowedValues(Roles.HouseholdManager, Roles.HouseholdMember, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string? Role { get; set; }
}
