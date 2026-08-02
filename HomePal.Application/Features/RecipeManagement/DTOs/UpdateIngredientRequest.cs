using HomePal.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class UpdateIngredientRequest
{
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }
    public IFormFile? Picture { get; set; }
    public bool RemovePicture { get; set; }
}
