using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Catalog.Mappers;

public static class OfferMapper
{
    public static OfferResponse ToResponse(this Offer offer, string? culture = null)
    {
        return new OfferResponse
        {
            Id = offer.Id,
            Name = offer.Name.Get(culture),
            Description = offer.Description?.Get(culture),
            Quantity = offer.Quantity,
            UnitId = offer.UnitId,
            UnitName = offer.Unit?.Name.Get(culture),
            UnitSymbol = offer.Unit?.Symbol?.Get(culture),
            OriginalPrice = offer.OriginalPrice,
            DiscountedPrice = offer.DiscountedPrice,
            ValidFrom = offer.ValidFrom,
            ValidTo = offer.ValidTo,
            CategoryId = offer.CategoryId,
            CategoryName = offer.Category?.Name.Get(culture),
            ImagePath = offer.ImagePath,
            SourceUrl = offer.SourceUrl,
            SupermarketId = offer.SupermarketId,
            SupermarketName = offer.Supermarket?.Name.Get(culture) ?? string.Empty,
            SupermarketLogoPath = offer.Supermarket?.LogoPath,
            SupermarketWebsiteUrl = offer.Supermarket?.WebsiteUrl,
            IsVerified = offer.IsVerified,
            CreatedAt = offer.CreatedAt
        };
    }

    public static Offer ToEntity(this CreateOfferRequest request)
    {
        return new Offer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Quantity = request.Quantity,
            UnitId = request.UnitId,
            OriginalPrice = request.OriginalPrice,
            DiscountedPrice = request.DiscountedPrice,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            CategoryId = request.CategoryId,
            ImagePath = request.ImagePath,
            SourceUrl = request.SourceUrl,
            SupermarketId = request.SupermarketId,
            IsVerified = request.IsVerified,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this Offer offer, UpdateOfferRequest request)
    {
        offer.Name = request.Name;
        offer.Description = request.Description;
        offer.Quantity = request.Quantity;
        offer.UnitId = request.UnitId;
        offer.OriginalPrice = request.OriginalPrice;
        offer.DiscountedPrice = request.DiscountedPrice;
        offer.ValidFrom = request.ValidFrom;
        offer.ValidTo = request.ValidTo;
        offer.CategoryId = request.CategoryId;
        offer.ImagePath = request.ImagePath;
        offer.SourceUrl = request.SourceUrl;
        offer.SupermarketId = request.SupermarketId;
        offer.IsVerified = request.IsVerified;
        offer.UpdatedAt = DateTime.UtcNow;
    }
}
