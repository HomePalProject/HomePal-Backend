using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Application.Features.RecipeManagement.Mappers;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagement.Services;

public class RecipeService : IRecipeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public RecipeService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<PaginatedList<RecipeResponse>>> GetRecipesAsync(RecipeFilterParams filter, CancellationToken cancellationToken = default)
    {
        var paginatedRecipes = await _unitOfWork.Recipes.GetFilteredPaginatedAsync(filter, cancellationToken);
        var responseItems = paginatedRecipes.Items.Select(r => r.ToResponse()).ToList();
        var result = PaginatedList<RecipeResponse>.Create(responseItems, paginatedRecipes.TotalCount, paginatedRecipes.PageNumber, filter.PageSize);

        return Result<PaginatedList<RecipeResponse>>.Ok(result, SuccessMessages.Recipe.GetAll);
    }

    public async Task<Result<RecipeResponse>> GetRecipeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.Recipes.GetByIdWithDetailsAsync(id, cancellationToken);
        if (recipe == null)
        {
            return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.NotFound, ResultStatus.NotFound);
        }

        return Result<RecipeResponse>.Ok(recipe.ToResponse(), SuccessMessages.Recipe.Get);
    }

    public async Task<Result<RecipeResponse>> CreateRecipeAsync(CreateRecipeRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Name == null || request.Name.Count == 0 || request.Name.All(n => string.IsNullOrWhiteSpace(n.Value)))
        {
            return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.NameRequired, ResultStatus.BadRequest);
        }

        if (request.ServingNum <= 0)
        {
            return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.ServingNumInvalid, ResultStatus.BadRequest);
        }

        if (request.Steps == null || request.Steps.Count == 0 || request.Steps.All(s => string.IsNullOrWhiteSpace(s.Value)))
        {
            return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.StepsRequired, ResultStatus.BadRequest);
        }

        // Validate ingredients
        if (request.Ingredients != null && request.Ingredients.Count > 0)
        {
            foreach (var item in request.Ingredients)
            {
                var ing = await _unitOfWork.Ingredients.GetByIdAsync(item.IngredientId, cancellationToken);
                if (ing == null)
                {
                    return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.InvalidIngredients, ResultStatus.BadRequest);
                }

                var unit = await _unitOfWork.MeasurementUnits.GetByIdAsync(item.MeasurementUnitId, cancellationToken);
                if (unit == null)
                {
                    return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.InvalidMeasurementUnit, ResultStatus.BadRequest);
                }
            }
        }

        // Validate preferences
        List<Preference> preferences = new();
        if (request.PreferenceIds != null && request.PreferenceIds.Count > 0)
        {
            foreach (var prefId in request.PreferenceIds)
            {
                var pref = await _unitOfWork.Preferences.GetByIdAsync(prefId, cancellationToken);
                if (pref == null)
                {
                    return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.InvalidPreferences, ResultStatus.BadRequest);
                }
                preferences.Add(pref);
            }
        }

        string? imageUrl = null;
        if (request.Image != null && request.Image.Length > 0)
        {
            try
            {
                imageUrl = await _fileStorageService.SaveFileAsync(request.Image, "recipes", cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return Result<RecipeResponse>.Fail(ex.Message, ResultStatus.BadRequest);
            }
        }

        var recipe = new Recipe
        {
            Name = request.Name,
            Description = request.Description,
            Steps = request.Steps,
            ServingNum = request.ServingNum,
            PrepTimeMinutes = request.PrepTimeMinutes,
            CookTimeMinutes = request.CookTimeMinutes,
            Difficulty = request.Difficulty,
            CaloriesPerServing = request.CaloriesPerServing,
            FatsPerServing = request.FatsPerServing,
            CarbsPerServing = request.CarbsPerServing,
            ProteinPerServing = request.ProteinPerServing,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            Preferences = preferences
        };

        if (request.Ingredients != null && request.Ingredients.Count > 0)
        {
            foreach (var item in request.Ingredients)
            {
                recipe.RecipeIngredients.Add(new RecipeIngredient
                {
                    IngredientId = item.IngredientId,
                    Amount = item.Amount,
                    MeasurementUnitId = item.MeasurementUnitId,
                    Notes = item.Notes
                });
            }
        }

        await _unitOfWork.Recipes.AddAsync(recipe, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdRecipe = await _unitOfWork.Recipes.GetByIdWithDetailsAsync(recipe.Id, cancellationToken);
        return Result<RecipeResponse>.Ok(createdRecipe!.ToResponse(), SuccessMessages.Recipe.Create, ResultStatus.Created);
    }

    public async Task<Result<RecipeResponse>> UpdateRecipeAsync(Guid id, UpdateRecipeRequest request, CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.Recipes.GetByIdWithDetailsAsync(id, cancellationToken);
        if (recipe == null)
        {
            return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.NotFound, ResultStatus.NotFound);
        }

        if (request.Name == null || request.Name.Count == 0 || request.Name.All(n => string.IsNullOrWhiteSpace(n.Value)))
        {
            return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.NameRequired, ResultStatus.BadRequest);
        }

        if (request.ServingNum <= 0)
        {
            return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.ServingNumInvalid, ResultStatus.BadRequest);
        }

        if (request.Steps == null || request.Steps.Count == 0 || request.Steps.All(s => string.IsNullOrWhiteSpace(s.Value)))
        {
            return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.StepsRequired, ResultStatus.BadRequest);
        }

        // Validate ingredients
        if (request.Ingredients != null && request.Ingredients.Count > 0)
        {
            foreach (var item in request.Ingredients)
            {
                var ing = await _unitOfWork.Ingredients.GetByIdAsync(item.IngredientId, cancellationToken);
                if (ing == null)
                {
                    return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.InvalidIngredients, ResultStatus.BadRequest);
                }

                var unit = await _unitOfWork.MeasurementUnits.GetByIdAsync(item.MeasurementUnitId, cancellationToken);
                if (unit == null)
                {
                    return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.InvalidMeasurementUnit, ResultStatus.BadRequest);
                }
            }
        }

        // Validate preferences
        List<Preference> preferences = new();
        if (request.PreferenceIds != null && request.PreferenceIds.Count > 0)
        {
            foreach (var prefId in request.PreferenceIds)
            {
                var pref = await _unitOfWork.Preferences.GetByIdAsync(prefId, cancellationToken);
                if (pref == null)
                {
                    return Result<RecipeResponse>.Fail(ErrorMessages.Recipe.InvalidPreferences, ResultStatus.BadRequest);
                }
                preferences.Add(pref);
            }
        }

        if (request.RemoveImage && !string.IsNullOrWhiteSpace(recipe.ImageUrl))
        {
            await _fileStorageService.DeleteFileAsync(recipe.ImageUrl);
            recipe.ImageUrl = null;
        }

        if (request.Image != null && request.Image.Length > 0)
        {
            if (!string.IsNullOrWhiteSpace(recipe.ImageUrl))
            {
                await _fileStorageService.DeleteFileAsync(recipe.ImageUrl);
            }
            try
            {
                recipe.ImageUrl = await _fileStorageService.SaveFileAsync(request.Image, "recipes", cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return Result<RecipeResponse>.Fail(ex.Message, ResultStatus.BadRequest);
            }
        }

        recipe.Name = request.Name;
        recipe.Description = request.Description;
        recipe.Steps = request.Steps;
        recipe.ServingNum = request.ServingNum;
        recipe.PrepTimeMinutes = request.PrepTimeMinutes;
        recipe.CookTimeMinutes = request.CookTimeMinutes;
        recipe.Difficulty = request.Difficulty;
        recipe.CaloriesPerServing = request.CaloriesPerServing;
        recipe.FatsPerServing = request.FatsPerServing;
        recipe.CarbsPerServing = request.CarbsPerServing;
        recipe.ProteinPerServing = request.ProteinPerServing;
        recipe.UpdatedAt = DateTime.UtcNow;

        // Update preferences
        recipe.Preferences.Clear();
        foreach (var p in preferences)
        {
            recipe.Preferences.Add(p);
        }

        // Update ingredients
        recipe.RecipeIngredients.Clear();
        if (request.Ingredients != null && request.Ingredients.Count > 0)
        {
            foreach (var item in request.Ingredients)
            {
                recipe.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = item.IngredientId,
                    Amount = item.Amount,
                    MeasurementUnitId = item.MeasurementUnitId,
                    Notes = item.Notes
                });
            }
        }

        _unitOfWork.Recipes.Update(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedRecipe = await _unitOfWork.Recipes.GetByIdWithDetailsAsync(recipe.Id, cancellationToken);
        return Result<RecipeResponse>.Ok(updatedRecipe!.ToResponse(), SuccessMessages.Recipe.Update);
    }

    public async Task<Result> DeleteRecipeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.Recipes.GetByIdAsync(id, cancellationToken);
        if (recipe == null)
        {
            return Result.Fail(ErrorMessages.Recipe.NotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(recipe.ImageUrl))
        {
            await _fileStorageService.DeleteFileAsync(recipe.ImageUrl);
        }

        _unitOfWork.Recipes.Remove(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Recipe.Delete);
    }
}
