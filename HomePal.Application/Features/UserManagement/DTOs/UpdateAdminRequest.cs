using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.UserManagement.DTOs;

public class UpdateAdminRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [EmailAddress(ErrorMessage = ErrorMessages.Validation.Email)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string FullName { get; set; } = string.Empty;

    public Gender Gender { get; set; } = Gender.Male;
    public DateOnly BirthDate { get; set; }
    public Guid? GovernorateId { get; set; }
    public Guid? CityId { get; set; }
    public string? PhoneNumber { get; set; }
}
