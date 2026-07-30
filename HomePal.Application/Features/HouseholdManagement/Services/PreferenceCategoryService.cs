using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Mappers;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Services;

public class PreferenceCategoryService : IPreferenceCategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public PreferenceCategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyCollection<PreferenceCategoryResponse>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.PreferenceCategories.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<PreferenceCategoryResponse>>.Ok(categories.ToResponseList(), SuccessMessages.Household.GetAllCategories);
    }

    public async Task<Result<IReadOnlyCollection<PreferenceCategoryResponse>>> SearchCategoriesAsync(string? query, CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.PreferenceCategories.SearchAsync(query ?? string.Empty, cancellationToken);
        return Result<IReadOnlyCollection<PreferenceCategoryResponse>>.Ok(categories.ToResponseList(), SuccessMessages.Household.SearchCategories);
    }

    public async Task<Result<PreferenceCategoryResponse>> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.PreferenceCategories.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
        {
            return Result<PreferenceCategoryResponse>.Fail(ErrorMessages.Household.CategoryNotFound, ResultStatus.NotFound);
        }

        return Result<PreferenceCategoryResponse>.Ok(category.ToResponse(), SuccessMessages.Household.GetCategory);
    }

    public async Task<Result<PreferenceCategoryResponse>> CreateCategoryAsync(Guid userId, CreatePreferenceCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var existingCategory = await _unitOfWork.PreferenceCategories.GetByNameAsync(request.Name, cancellationToken);
        if (existingCategory != null)
        {
            return Result<PreferenceCategoryResponse>.Fail(ErrorMessages.Household.CategoryAlreadyExists, ResultStatus.BadRequest);
        }

        var category = request.ToEntity();
        await _unitOfWork.PreferenceCategories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PreferenceCategoryResponse>.Ok(category.ToResponse(), SuccessMessages.Household.AddCategory, ResultStatus.Created);
    }

    public async Task<Result<PreferenceCategoryResponse>> UpdateCategoryAsync(Guid userId, Guid categoryId, UpdatePreferenceCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.PreferenceCategories.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
        {
            return Result<PreferenceCategoryResponse>.Fail(ErrorMessages.Household.CategoryNotFound, ResultStatus.NotFound);
        }

        var existingWithName = await _unitOfWork.PreferenceCategories.GetByNameAsync(request.Name, cancellationToken);
        if (existingWithName != null && existingWithName.Id != categoryId)
        {
            return Result<PreferenceCategoryResponse>.Fail(ErrorMessages.Household.CategoryAlreadyExists, ResultStatus.BadRequest);
        }

        category.Name = request.Name.Trim();
        category.Description = request.Description?.Trim();
        category.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.PreferenceCategories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PreferenceCategoryResponse>.Ok(category.ToResponse(), SuccessMessages.Household.UpdateCategory);
    }

    public async Task<Result> DeleteCategoryAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.PreferenceCategories.GetByIdWithPreferencesAsync(categoryId, cancellationToken);
        if (category == null)
        {
            return Result.Fail(ErrorMessages.Household.CategoryNotFound, ResultStatus.NotFound);
        }

        if (category.Preferences.Count > 0)
        {
            return Result.Fail(ErrorMessages.Household.CategoryHasPreferences, ResultStatus.BadRequest);
        }

        _unitOfWork.PreferenceCategories.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Household.DeleteCategory);
    }
}
