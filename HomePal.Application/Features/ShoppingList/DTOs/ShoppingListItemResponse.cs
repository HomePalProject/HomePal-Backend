namespace HomePal.Application.Features.ShoppingList.DTOs;

public class ShoppingListItemResponse
{
    public Guid Id { get; set; }
    public Guid ShoppingListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public decimal? Price { get; set; }

    public Guid? UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? UnitSymbol { get; set; }

    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public Guid? OfferId { get; set; }
    public string? OfferImagePath { get; set; }
    public string? SupermarketName { get; set; }
    public string? SupermarketLogoPath { get; set; }

    public bool IsPurchased { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
