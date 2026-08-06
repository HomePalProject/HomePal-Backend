using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.PantryManagement.Mappers;

public static class PantryItemMapper
{
    public static PantryItemResponse ToResponse(this PantryItem item)
    {
        return new PantryItemResponse
        {
            Id = item.Id,
            PantryId = item.PantryId,
            Name = item.Name,
            ExpireDate = item.ExpireDate,
            Quantity = item.Quantity,
            MeasuringUnitId = item.MeasuringUnitId,
            MeasuringUnitName = item.MeasuringUnit?.Name,
            MeasuringUnitSymbol = item.MeasuringUnit?.Symbol,
            CategoryId = item.CategoryId,
            CategoryName = item.Category?.Name,
            CategoryImagePath = item.Category?.ImagePath,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    public static PantryItem ToEntity(this CreatePantryItemRequest request, Guid pantryId)
    {
        return new PantryItem
        {
            Id = Guid.NewGuid(),
            PantryId = pantryId,
            Name = request.Name.Trim(),
            ExpireDate = request.ExpireDate,
            Quantity = request.Quantity,
            MeasuringUnitId = request.MeasuringUnitId,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
