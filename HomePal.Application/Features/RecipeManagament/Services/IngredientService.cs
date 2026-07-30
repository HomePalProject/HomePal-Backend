using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.RecipeManagament.DTOs;
using HomePal.Application.Features.RecipeManagament.Interfaces;
using HomePal.Application.Features.RecipeManagament.Mappers;
using HomePal.Domain.Entities.Recipe;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagament.Services;

public class IngredientService : IIngredientService
{
    private readonly IUnitOfWork _unitOfWork;

    public IngredientService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<IngredientResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var ingredients = await _unitOfWork.Ingredients
            .GetAllAsync(cancellationToken);

        return Result<IReadOnlyList<IngredientResponse>>.Ok(
            ingredients.ToResponse());
    }

    public async Task<Result<IngredientResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var ingredient = await _unitOfWork.Ingredients
            .GetByIdAsync(id, cancellationToken);

        if (ingredient is null)
        {
            return Result<IngredientResponse>.Fail(
                "Ingredient.NotFound",
                ResultStatus.NotFound);
        }

        return Result<IngredientResponse>.Ok(
            ingredient.ToResponse());
    }

    public async Task<Result<IngredientResponse>> CreateAsync(
        CreateIngredientRequest request,
        CancellationToken cancellationToken = default)
    {
        var existingIngredient = await _unitOfWork.Ingredients
            .GetByNameAsync(request.Name, cancellationToken);

        if (existingIngredient is not null)
        {
            return Result<IngredientResponse>.Fail(
                "Ingredient.AlreadyExists",
                ResultStatus.Conflict);
        }

        var ingredient = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            DefaultUnit = request.DefaultUnit,
            Category = request.Category
        };

        await _unitOfWork.Ingredients.AddAsync(
            ingredient,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<IngredientResponse>.Ok(
            ingredient.ToResponse(),
            "Ingredient.CreateSuccess",
            ResultStatus.Created);
    }

    public async Task<Result> UpdateAsync(
        Guid id,
        UpdateIngredientRequest request,
        CancellationToken cancellationToken = default)
    {
        var ingredient = await _unitOfWork.Ingredients
            .GetByIdAsync(id, cancellationToken);

        if (ingredient is null)
        {
            return Result.Fail(
                "Ingredient.NotFound",
                ResultStatus.NotFound);
        }

        var existingIngredient = await _unitOfWork.Ingredients
            .GetByNameAsync(request.Name, cancellationToken);

        if (existingIngredient is not null &&
            existingIngredient.Id != id)
        {
            return Result.Fail(
                "Ingredient.AlreadyExists",
                ResultStatus.Conflict);
        }

        ingredient.Name = request.Name.Trim();
        ingredient.DefaultUnit = request.DefaultUnit;
        ingredient.Category = request.Category;

        _unitOfWork.Ingredients.Update(ingredient);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Ok(
            "Ingredient.UpdateSuccess");
    }

    public async Task<Result> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var ingredient = await _unitOfWork.Ingredients
            .GetByIdAsync(id, cancellationToken);

        if (ingredient is null)
        {
            return Result.Fail(
                "Ingredient.NotFound",
                ResultStatus.NotFound);
        }

        var usedInRecipes = await _unitOfWork.Recipes
            .FindAsync(r => r.Ingredients.Any(i => i.IngredientId == id),
                cancellationToken);

        if (usedInRecipes.Any())
        {
            return Result.Fail(
                "Ingredient.InUse",
                ResultStatus.Conflict);
        }

        _unitOfWork.Ingredients.Remove(ingredient);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Ok(
            "Ingredient.DeleteSuccess");
    }
}