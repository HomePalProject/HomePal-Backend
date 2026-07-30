using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.PantryManagement.DTOs;

public class CreatePantryItemRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(150, MinimumLength = 2, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string Name { get; set; } = string.Empty;

    public DateTime? ExpireDate { get; set; }

    [Range(0.01, 999999.99, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(50, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string MeasuringUnit { get; set; } = string.Empty;

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    [StringLength(100, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public string Category { get; set; } = string.Empty;
}
