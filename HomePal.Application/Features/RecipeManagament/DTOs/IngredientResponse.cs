using HomePal.Domain.Enums.RecipeEnums;

namespace HomePal.Application.Features.RecipeManagament.DTOs;

public class IngredientResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Amount { get; set; }

    public MeasurementUnit Unit { get; set; }
}