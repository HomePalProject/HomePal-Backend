using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Mappers;

public static class PreferenceMapper
{
    public static PreferenceResponse ToResponse(this Preference preference)
    {
        return new PreferenceResponse
        {
            Id = preference.Id,
            Name = preference.Name,
            Description = preference.Description,
            CategoryId = preference.CategoryId,
            CategoryName = preference.Category?.Name ?? string.Empty,
            CreatedAt = preference.CreatedAt,
            UpdatedAt = preference.UpdatedAt
        };
    }

    public static IReadOnlyCollection<PreferenceResponse> ToResponseList(this IEnumerable<Preference> preferences)
    {
        return preferences.Select(p => p.ToResponse()).ToList();
    }
}
