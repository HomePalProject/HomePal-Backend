using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.HouseholdManagement.Mappers;

public static class PreferenceCategoryMapper
{
    public static PreferenceCategoryResponse ToResponse(this PreferenceCategory category)
    {
        return new PreferenceCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public static IReadOnlyCollection<PreferenceCategoryResponse> ToResponseList(this IEnumerable<PreferenceCategory> categories)
    {
        return categories.Select(c => c.ToResponse()).ToList();
    }

    public static PreferenceCategory ToEntity(this CreatePreferenceCategoryRequest request)
    {
        return new PreferenceCategory
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
