using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Common;

namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class AddPreferenceRequest
{
    [Required]
    public List<LocalizedItem> Name { get; set; } = new();

    public List<LocalizedItem>? Description { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}


