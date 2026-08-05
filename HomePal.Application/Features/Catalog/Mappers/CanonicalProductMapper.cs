using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Catalog.Mappers;

public static class CanonicalProductMapper
{
    public static CanonicalProductResponse ToResponse(this CanonicalProduct product, string? culture = null)
    {
        return new CanonicalProductResponse
        {
            Id = product.Id,
            Name = product.Name.Get(culture),
            Description = product.Description?.Get(culture),
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name.Get(culture),
            ImagePath = product.ImagePath,
            OffersCount = product.Offers?.Count ?? 0,
            CreatedAt = product.CreatedAt
        };
    }

    public static CanonicalProduct ToEntity(this CreateCanonicalProductRequest request)
    {
        return new CanonicalProduct
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            ImagePath = request.ImagePath,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this CanonicalProduct product, UpdateCanonicalProductRequest request)
    {
        product.Name = request.Name;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;
        product.ImagePath = request.ImagePath;
        product.UpdatedAt = DateTime.UtcNow;
    }
}
