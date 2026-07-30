using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Scan.DTOs;

public class ScanImageRequest
{
    [Required(ErrorMessage = ErrorMessages.Scan.NoImageUploaded)]
    public IFormFile Image { get; set; } = null!;
}
