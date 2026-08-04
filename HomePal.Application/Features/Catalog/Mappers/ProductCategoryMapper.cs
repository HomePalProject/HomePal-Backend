using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Catalog.Mappers;

public static class ProductCategoryMapper
{
    public static ProductCategoryResponse ToResponse(this ProductCategory category, string? culture = null)
    {
        return new ProductCategoryResponse
        {
            Id = category.Id,
            Name = category.Name.Get(culture),
            Description = category.Description?.Get(culture),
            ImagePath = category.ImagePath,
            CreatedAt = category.CreatedAt
        };
    }

    public static ProductCategory ToEntity(this CreateProductCategoryRequest request)
    {
        return new ProductCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ImagePath = request.ImagePath,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this ProductCategory category, UpdateProductCategoryRequest request)
    {
        category.Name = request.Name;
        category.Description = request.Description;
        category.ImagePath = request.ImagePath ?? category.ImagePath;
        category.UpdatedAt = DateTime.UtcNow;
    }
}
