using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.RecipeManagement.Mappers;

public static class MeasurementUnitMapper
{
    public static MeasurementUnitResponse ToResponse(this MeasurementUnit unit)
    {
        return new MeasurementUnitResponse
        {
            Id = unit.Id,
            Name = unit.Name,
            Symbol = unit.Symbol,
            Description = unit.Description,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }
}
