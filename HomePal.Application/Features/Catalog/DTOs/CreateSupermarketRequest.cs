using System.ComponentModel.DataAnnotations;
using HomePal.Domain.Common;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Catalog.DTOs;

public class CreateSupermarketRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public List<LocalizedItem> Name { get; set; } = new();

    public string? LogoPath { get; set; }
    public string? Address { get; set; }
    public string? WebsiteUrl { get; set; }
}
