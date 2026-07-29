using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Auth.DTOs;

public class UploadProfileImageRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public IFormFile Image { get; set; } = null!;
}
