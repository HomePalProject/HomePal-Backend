using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.PantryManagement.DTOs;

public class CreatePantryItemRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateTime? ExpireDate { get; set; }

    [Range(0.01, 999999.99, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public Guid MeasuringUnitId { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public Guid CategoryId { get; set; }
}
