using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.DTOs;

public class UploadProductImageRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public IFormFile Image { get; set; } = null!;
}
