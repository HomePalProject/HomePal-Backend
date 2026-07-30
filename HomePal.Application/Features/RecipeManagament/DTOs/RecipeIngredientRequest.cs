using HomePal.Domain.Enums.RecipeEnums;
using System.ComponentModel.DataAnnotations;

namespace HomePal.Application.Features.RecipeManagament.DTOs;

public class RecipeIngredientRequest
{
    [Required]
    public Guid IngredientId { get; set; }

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Amount { get; set; }

    [Required]
    public MeasurementUnit Unit { get; set; }
}