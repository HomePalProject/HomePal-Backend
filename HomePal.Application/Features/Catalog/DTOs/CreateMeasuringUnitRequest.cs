using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Common;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Catalog.DTOs;

public class CreateMeasuringUnitRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public List<LocalizedItem> Name { get; set; } = new();

    public List<LocalizedItem> Symbol { get; set; } = new();
}
