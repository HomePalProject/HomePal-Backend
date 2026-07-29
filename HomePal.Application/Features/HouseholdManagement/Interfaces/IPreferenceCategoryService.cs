using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IPreferenceCategoryService
{
    Task<Result<IReadOnlyCollection<PreferenceCategoryResponse>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PreferenceCategoryResponse>>> SearchCategoriesAsync(string? query, CancellationToken cancellationToken = default);
    Task<Result<PreferenceCategoryResponse>> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<PreferenceCategoryResponse>> CreateCategoryAsync(Guid userId, CreatePreferenceCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<PreferenceCategoryResponse>> UpdateCategoryAsync(Guid userId, Guid categoryId, UpdatePreferenceCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteCategoryAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken = default);
}
