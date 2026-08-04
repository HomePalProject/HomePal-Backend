using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Common;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Catalog.DTOs;

public class CreateProductCategoryRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public List<LocalizedItem> Name { get; set; } = new();

    public List<LocalizedItem>? Description { get; set; }
    public string? ImagePath { get; set; }
}
