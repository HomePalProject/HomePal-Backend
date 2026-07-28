using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class AssignPreferencesRequest
{
    [Required]
    public List<Guid> PreferenceIds { get; set; } = new();
}
