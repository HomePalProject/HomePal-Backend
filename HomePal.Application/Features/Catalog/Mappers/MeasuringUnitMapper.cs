using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Catalog.Mappers;

public static class MeasuringUnitMapper
{
    public static MeasuringUnitResponse ToResponse(this MeasuringUnit unit, string? culture = null)
    {
        return new MeasuringUnitResponse
        {
            Id = unit.Id,
            Name = unit.Name.Get(culture),
            Symbol = unit.Symbol?.Get(culture),
            CreatedAt = unit.CreatedAt
        };
    }

    public static MeasuringUnit ToEntity(this CreateMeasuringUnitRequest request)
    {
        return new MeasuringUnit
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Symbol = request.Symbol,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this MeasuringUnit unit, UpdateMeasuringUnitRequest request)
    {
        unit.Name = request.Name;
        unit.Symbol = request.Symbol;
        unit.UpdatedAt = DateTime.UtcNow;
    }
}
