using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Application.Features.RecipeManagement.Mappers;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagement.Services;

public class IngredientService : IIngredientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public IngredientService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<PaginatedList<IngredientResponse>>> GetIngredientsAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default)
    {
        var paginatedIngredients = await _unitOfWork.Ingredients.GetPaginatedAsync(request, search, cancellationToken);
        var responseItems = paginatedIngredients.Items.Select(i => i.ToResponse()).ToList();
        var result = PaginatedList<IngredientResponse>.Create(responseItems, paginatedIngredients.TotalCount, paginatedIngredients.PageNumber, request.PageSize);

        return Result<PaginatedList<IngredientResponse>>.Ok(result, SuccessMessages.Ingredient.GetAll);
    }

    public async Task<Result<IngredientResponse>> GetIngredientByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ingredient = await _unitOfWork.Ingredients.GetByIdAsync(id, cancellationToken);
        if (ingredient == null)
        {
            return Result<IngredientResponse>.Fail(ErrorMessages.Ingredient.NotFound, ResultStatus.NotFound);
        }

        return Result<IngredientResponse>.Ok(ingredient.ToResponse(), SuccessMessages.Ingredient.Get);
    }

    public async Task<Result<IngredientResponse>> CreateIngredientAsync(CreateIngredientRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Name == null || request.Name.Count == 0 || request.Name.All(n => string.IsNullOrWhiteSpace(n.Value)))
        {
            return Result<IngredientResponse>.Fail(ErrorMessages.Ingredient.NameRequired, ResultStatus.BadRequest);
        }

        string? pictureUrl = null;
        if (request.Picture != null && request.Picture.Length > 0)
        {
            try
            {
                pictureUrl = await _fileStorageService.SaveFileAsync(request.Picture, "ingredients", cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return Result<IngredientResponse>.Fail(ex.Message, ResultStatus.BadRequest);
            }
        }

        var ingredient = new Ingredient
        {
            Name = request.Name,
            Description = request.Description,
            PictureUrl = pictureUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Ingredients.AddAsync(ingredient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IngredientResponse>.Ok(ingredient.ToResponse(), SuccessMessages.Ingredient.Create, ResultStatus.Created);
    }

    public async Task<Result<IngredientResponse>> UpdateIngredientAsync(Guid id, UpdateIngredientRequest request, CancellationToken cancellationToken = default)
    {
        var ingredient = await _unitOfWork.Ingredients.GetByIdAsync(id, cancellationToken);
        if (ingredient == null)
        {
            return Result<IngredientResponse>.Fail(ErrorMessages.Ingredient.NotFound, ResultStatus.NotFound);
        }

        if (request.Name == null || request.Name.Count == 0 || request.Name.All(n => string.IsNullOrWhiteSpace(n.Value)))
        {
            return Result<IngredientResponse>.Fail(ErrorMessages.Ingredient.NameRequired, ResultStatus.BadRequest);
        }

        if (request.RemovePicture && !string.IsNullOrWhiteSpace(ingredient.PictureUrl))
        {
            await _fileStorageService.DeleteFileAsync(ingredient.PictureUrl);
            ingredient.PictureUrl = null;
        }

        if (request.Picture != null && request.Picture.Length > 0)
        {
            if (!string.IsNullOrWhiteSpace(ingredient.PictureUrl))
            {
                await _fileStorageService.DeleteFileAsync(ingredient.PictureUrl);
            }
            try
            {
                ingredient.PictureUrl = await _fileStorageService.SaveFileAsync(request.Picture, "ingredients", cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return Result<IngredientResponse>.Fail(ex.Message, ResultStatus.BadRequest);
            }
        }

        ingredient.Name = request.Name;
        ingredient.Description = request.Description;
        ingredient.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Ingredients.Update(ingredient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IngredientResponse>.Ok(ingredient.ToResponse(), SuccessMessages.Ingredient.Update);
    }

    public async Task<Result> DeleteIngredientAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ingredient = await _unitOfWork.Ingredients.GetByIdAsync(id, cancellationToken);
        if (ingredient == null)
        {
            return Result.Fail(ErrorMessages.Ingredient.NotFound, ResultStatus.NotFound);
        }

        var isUsed = await _unitOfWork.Ingredients.IsUsedInRecipesAsync(id, cancellationToken);
        if (isUsed)
        {
            return Result.Fail(ErrorMessages.Ingredient.InUse, ResultStatus.BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(ingredient.PictureUrl))
        {
            await _fileStorageService.DeleteFileAsync(ingredient.PictureUrl);
        }

        _unitOfWork.Ingredients.Remove(ingredient);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Ingredient.Delete);
    }
}
