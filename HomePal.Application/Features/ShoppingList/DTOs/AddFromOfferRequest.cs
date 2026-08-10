using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.ShoppingList.DTOs;

public class AddFromOfferRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public Guid OfferId { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public double? CustomQuantity { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public int? PortionCount { get; set; }

    public string? Notes { get; set; }
}
