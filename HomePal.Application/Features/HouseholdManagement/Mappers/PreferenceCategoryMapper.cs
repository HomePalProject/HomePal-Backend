using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Mappers;


public static class PreferenceCategoryMapper
{
    public static PreferenceCategoryResponse ToResponse(this PreferenceCategory category, string? culture = null)
    {
        return new PreferenceCategoryResponse
        {
            Id = category.Id,
            Name = category.Name.Get(culture),
            Description = category.Description?.Get(culture),
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }


    public static IReadOnlyCollection<PreferenceCategoryResponse> ToResponseList(this IEnumerable<PreferenceCategory> categories, string? culture = null)
    {
        return categories.Select(c => c.ToResponse(culture)).ToList();
    }

    public static PreferenceCategory ToEntity(this CreatePreferenceCategoryRequest request)
    {
        return new PreferenceCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
    }
}

