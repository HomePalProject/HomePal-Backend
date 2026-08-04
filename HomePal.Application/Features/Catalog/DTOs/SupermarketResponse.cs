namespace HomePal.Application.Features.Catalog.DTOs;

public class SupermarketResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoPath { get; set; }
    public string? Address { get; set; }
    public string? WebsiteUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
