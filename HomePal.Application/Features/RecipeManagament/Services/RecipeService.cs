using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.RecipeManagament.DTOs;
using HomePal.Application.Features.RecipeManagament.Interfaces;
using HomePal.Application.Features.RecipeManagament.Mappers;
using HomePal.Domain.Entities.Recipe;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagament.Services;

public class RecipeService : IRecipeService
{
    private readonly IUnitOfWork _unitOfWork;

    public RecipeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<RecipeSummaryResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var recipes = await _unitOfWork.Recipes
            .GetAllAsync(cancellationToken);

        return Result<IReadOnlyList<RecipeSummaryResponse>>.Ok(
            recipes.ToSummaryResponse(),
            "Recipe.GetAllSuccess");
    }

    public async Task<Result<RecipeResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.Recipes
            .GetByIdWithDetailsAsync(id, cancellationToken);

        if (recipe is null)
        {
            return Result<RecipeResponse>.Fail(
                "Recipe.NotFound",
                ResultStatus.NotFound);
        }

        return Result<RecipeResponse>.Ok(
            recipe.ToResponse(),
            "Recipe.GetSuccess");
    }

    public async Task<Result<RecipeResponse>> CreateAsync(
     CreateRecipeRequest request,
     CancellationToken cancellationToken = default)
    {
        var existingRecipe = await _unitOfWork.Recipes
            .GetByNameAsync(request.Name, cancellationToken);

        if (existingRecipe is not null)
        {
            return Result<RecipeResponse>.Fail(
                "Recipe.AlreadyExists",
                ResultStatus.Conflict);
        }

        if (request.Ingredients
            .GroupBy(i => i.IngredientId)
            .Any(g => g.Count() > 1))
        {
            return Result<RecipeResponse>.Fail(
                "Recipe.DuplicateIngredient",
                ResultStatus.BadRequest);
        }

        if (request.Steps
            .GroupBy(s => s.Order)
            .Any(g => g.Count() > 1))
        {
            return Result<RecipeResponse>.Fail(
                "Recipe.DuplicateStepOrder",
                ResultStatus.BadRequest);
        }

        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Difficulty = request.Difficulty,
            TimeToMake = request.TimeToMake,
            Servings = request.Servings,
            ImageUrl = request.ImageUrl
        };

        foreach (var ingredientRequest in request.Ingredients)
        {
            var ingredient = await _unitOfWork.Ingredients
                .GetByIdAsync(
                    ingredientRequest.IngredientId,
                    cancellationToken);

            if (ingredient is null)
            {
                return Result<RecipeResponse>.Fail(
                    $"Ingredient '{ingredientRequest.IngredientId}' not found.",
                    ResultStatus.NotFound);
            }

            recipe.Ingredients.Add(new RecipeIngredient
            {
                RecipeId = recipe.Id,
                IngredientId = ingredient.Id,
                Amount = ingredientRequest.Amount,
                Unit = ingredientRequest.Unit
            });
        }

        foreach (var step in request.Steps.OrderBy(s => s.Order))
        {
            recipe.Steps.Add(new RecipeStep
            {
                Id = Guid.NewGuid(),
                RecipeId = recipe.Id,
                StepOrder = step.Order,
                Description = step.Description.Trim()
            });
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await _unitOfWork.Recipes.AddAsync(
                recipe,
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(
                cancellationToken);

            recipe = await _unitOfWork.Recipes
                .GetByIdWithDetailsAsync(
                    recipe.Id,
                    cancellationToken) ?? recipe;

            return Result<RecipeResponse>.Ok(
                recipe.ToResponse(),
                "Recipe.CreateSuccess",
                ResultStatus.Created);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                cancellationToken);

            return Result<RecipeResponse>.Fail(
                ErrorMessages.Server.InternalError,
                ResultStatus.Failure);
        }
    }

    public async Task<Result> UpdateAsync(
    Guid id,
    UpdateRecipeRequest request,
    CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.Recipes
            .GetByIdWithDetailsAsync(id, cancellationToken);

        if (recipe is null)
        {
            return Result.Fail(
                "Recipe.NotFound",
                ResultStatus.NotFound);
        }

        var existingRecipe = await _unitOfWork.Recipes
            .GetByNameAsync(request.Name, cancellationToken);

        if (existingRecipe is not null &&
            existingRecipe.Id != id)
        {
            return Result.Fail(
                "Recipe.AlreadyExists",
                ResultStatus.Conflict);
        }

        if (request.Ingredients
            .GroupBy(i => i.IngredientId)
            .Any(g => g.Count() > 1))
        {
            return Result.Fail(
                "Recipe.DuplicateIngredient",
                ResultStatus.BadRequest);
        }

        if (request.Steps
            .GroupBy(s => s.Order)
            .Any(g => g.Count() > 1))
        {
            return Result.Fail(
                "Recipe.DuplicateStepOrder",
                ResultStatus.BadRequest);
        }

        recipe.Name = request.Name.Trim();
        recipe.Description = request.Description?.Trim();
        recipe.Difficulty = request.Difficulty;
        recipe.TimeToMake = request.TimeToMake;
        recipe.Servings = request.Servings;
        recipe.ImageUrl = request.ImageUrl;

        recipe.Ingredients.Clear();

        foreach (var ingredientRequest in request.Ingredients)
        {
            var ingredient = await _unitOfWork.Ingredients
                .GetByIdAsync(
                    ingredientRequest.IngredientId,
                    cancellationToken);

            if (ingredient is null)
            {
                return Result.Fail(
                    $"Ingredient '{ingredientRequest.IngredientId}' not found.",
                    ResultStatus.NotFound);
            }

            recipe.Ingredients.Add(new RecipeIngredient
            {
                RecipeId = recipe.Id,
                IngredientId = ingredient.Id,
                Amount = ingredientRequest.Amount,
                Unit = ingredientRequest.Unit
            });
        }

        recipe.Steps.Clear();

        foreach (var step in request.Steps.OrderBy(s => s.Order))
        {
            recipe.Steps.Add(new RecipeStep
            {
                Id = Guid.NewGuid(),
                RecipeId = recipe.Id,
                StepOrder = step.Order,
                Description = step.Description.Trim()
            });
        }

        _unitOfWork.Recipes.Update(recipe);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Recipe.UpdateSuccess");
    }


    public async Task<Result> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        var recipe = await _unitOfWork.Recipes
            .GetByIdAsync(id, cancellationToken);

        if (recipe is null)
        {
            return Result.Fail(
                "Recipe.NotFound",
                ResultStatus.NotFound);
        }

        _unitOfWork.Recipes.Remove(recipe);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Recipe.DeleteSuccess");
    }


}