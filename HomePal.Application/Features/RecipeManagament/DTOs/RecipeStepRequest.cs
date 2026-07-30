using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.RecipeManagament.DTOs;

public class RecipeStepRequest
{
    [Range(1, int.MaxValue)]
    public int Order { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 2)]
    public string Description { get; set; } = null!;
}