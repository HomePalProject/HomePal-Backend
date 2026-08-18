namespace HomePal.Application.Features.Catalog.DTOs;

public class OfferResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Quantity { get; set; }
    public Guid? UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitSymbol { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? ImagePath { get; set; }
    public string? SourceUrl { get; set; }
    public Guid SupermarketId { get; set; }
    public string SupermarketName { get; set; } = string.Empty;
    public string? SupermarketLogoPath { get; set; }
    public string? SupermarketWebsiteUrl { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}
