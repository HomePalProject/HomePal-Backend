using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.HouseholdManagement.DTOs;

public class CreatePreferenceCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }
}
