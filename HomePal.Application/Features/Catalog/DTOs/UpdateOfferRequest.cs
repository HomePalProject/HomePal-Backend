using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Common;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Catalog.DTOs;

public class UpdateOfferRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public List<LocalizedItem> Name { get; set; } = new();

    public List<LocalizedItem>? Description { get; set; }

    [Range(0.0001, double.MaxValue, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public double Quantity { get; set; }

    public Guid? UnitId { get; set; }

    [Range(0, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public decimal OriginalPrice { get; set; }

    [Range(0, (double)decimal.MaxValue, ErrorMessage = ErrorMessages.Validation.InvalidValue)]
    public decimal DiscountedPrice { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public Guid? CategoryId { get; set; }
    public string? ImagePath { get; set; }

    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public Guid SupermarketId { get; set; }

    public bool IsVerified { get; set; } = true;
}
