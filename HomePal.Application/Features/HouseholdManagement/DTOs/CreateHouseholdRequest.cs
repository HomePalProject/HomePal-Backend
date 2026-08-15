using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class CreateHouseholdRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string? Address { get; set; }

    public Guid? GovernorateId { get; set; }
    public Guid? CityId { get; set; }
}
