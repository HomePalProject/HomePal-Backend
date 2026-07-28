using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class UpdateHouseholdRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string? Governorate { get; set; }

    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string? City { get; set; }
}
