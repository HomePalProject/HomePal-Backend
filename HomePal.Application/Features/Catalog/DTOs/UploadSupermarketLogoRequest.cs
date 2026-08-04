using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.DTOs;

public class UploadSupermarketLogoRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public IFormFile Image { get; set; } = null!;
}
