using HomePal.Application.Features.Locations.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Locations.Mappers;

public static class LocationMapper
{
    public static GovernorateResponse ToResponse(this Governorate governorate, string? culture = null)
    {
        return new GovernorateResponse
        {
            Id = governorate.Id,
            Code = governorate.Code,
            Name = governorate.Name.Get(culture),
            Latitude = governorate.Latitude,
            Longitude = governorate.Longitude,
            CitiesCount = governorate.Cities?.Count ?? 0
        };
    }

    public static GovernorateDetailResponse ToDetailResponse(this Governorate governorate, string? culture = null)
    {
        return new GovernorateDetailResponse
        {
            Id = governorate.Id,
            Code = governorate.Code,
            Name = governorate.Name.Get(culture),
            Latitude = governorate.Latitude,
            Longitude = governorate.Longitude,
            Cities = governorate.Cities?.Select(c => c.ToResponse(culture)).ToList() ?? new List<CityResponse>()
        };
    }

    public static CityResponse ToResponse(this City city, string? culture = null)
    {
        return new CityResponse
        {
            Id = city.Id,
            Name = city.Name.Get(culture),
            GovernorateId = city.GovernorateId,
            GovernorateCode = city.GovernorateCode,
            GovernorateName = city.Governorate?.Name.Get(culture),
            Latitude = city.Latitude,
            Longitude = city.Longitude
        };
    }
}
