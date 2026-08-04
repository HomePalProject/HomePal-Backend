namespace HomePal.Application.Features.Catalog.DTOs;

public class ProductCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
}
