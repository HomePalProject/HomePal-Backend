using HomePal.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.RecipeManagement.DTOs;

public class CreateIngredientRequest
{
    public List<LocalizedItem> Name { get; set; } = new();
    public List<LocalizedItem>? Description { get; set; }
    public IFormFile? Picture { get; set; }
}
