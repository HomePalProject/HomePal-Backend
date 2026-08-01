using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Mappers;


public static class PreferenceMapper
{
    public static PreferenceResponse ToResponse(this Preference preference, string? culture = null)
    {
        return new PreferenceResponse
        {
            Id = preference.Id,
            Name = preference.Name.Get(culture),
            Description = preference.Description?.Get(culture),
            CategoryId = preference.CategoryId,
            CategoryName = preference.Category?.Name.Get(culture) ?? string.Empty,
            CreatedAt = preference.CreatedAt,
            UpdatedAt = preference.UpdatedAt
        };
    }


    public static IReadOnlyCollection<PreferenceResponse> ToResponseList(this IEnumerable<Preference> preferences, string? culture = null)
    {
        return preferences.Select(p => p.ToResponse(culture)).ToList();
    }
}

