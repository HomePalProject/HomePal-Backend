using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.ShoppingList.DTOs;

public class CreateShoppingListItemRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public string Name { get; set; } = string.Empty;

    [Range(0.0001, double.MaxValue, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public double Quantity { get; set; } = 1;

    [Range(0, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public decimal? Price { get; set; }

    public Guid? UnitId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Notes { get; set; }
}
