namespace HomePal.Application.Features.Catalog.DTOs;

public class CanonicalProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CroppedImagePath { get; set; }
    public int OffersCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
