using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.PantryManagement.Mappers;

public static class PantryItemMapper
{
    public static PantryItemResponse ToResponse(this PantryItem entity)
    {
        return new PantryItemResponse
        {
            Id = entity.Id,
            PantryId = entity.PantryId,
            Name = entity.Name,
            ExpireDate = entity.ExpireDate,
            Quantity = entity.Quantity,
            MeasuringUnit = entity.MeasuringUnit,
            Category = entity.Category,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static PantryItem ToEntity(this CreatePantryItemRequest request, Guid pantryId)
    {
        return new PantryItem
        {
            PantryId = pantryId,
            Name = request.Name.Trim(),
            ExpireDate = request.ExpireDate,
            Quantity = request.Quantity,
            MeasuringUnit = request.MeasuringUnit.Trim(),
            Category = request.Category.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
