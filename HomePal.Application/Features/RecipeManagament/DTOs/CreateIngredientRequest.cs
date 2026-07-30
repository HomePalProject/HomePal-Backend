using HomePal.Domain.Enums.RecipeEnums;
using System.ComponentModel.DataAnnotations;

public class CreateIngredientRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    public MeasurementUnit DefaultUnit { get; set; }

    [Required]
    public IngredientCategory Category { get; set; }
}