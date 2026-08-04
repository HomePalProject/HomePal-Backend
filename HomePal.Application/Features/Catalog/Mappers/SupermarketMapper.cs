using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Catalog.Mappers;

public static class SupermarketMapper
{
    public static SupermarketResponse ToResponse(this Supermarket supermarket, string? culture = null)
    {
        return new SupermarketResponse
        {
            Id = supermarket.Id,
            Name = supermarket.Name.Get(culture),
            LogoPath = supermarket.LogoPath,
            Address = supermarket.Address,
            WebsiteUrl = supermarket.WebsiteUrl,
            CreatedAt = supermarket.CreatedAt
        };
    }

    public static Supermarket ToEntity(this CreateSupermarketRequest request)
    {
        return new Supermarket
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            LogoPath = request.LogoPath,
            Address = request.Address,
            WebsiteUrl = request.WebsiteUrl,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this Supermarket supermarket, UpdateSupermarketRequest request)
    {
        supermarket.Name = request.Name;
        supermarket.LogoPath = request.LogoPath;
        supermarket.Address = request.Address;
        supermarket.WebsiteUrl = request.WebsiteUrl;
        supermarket.UpdatedAt = DateTime.UtcNow;
    }
}
