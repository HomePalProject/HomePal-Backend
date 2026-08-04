using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.Catalog.Mappers;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public ProductCategoryService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<IReadOnlyCollection<ProductCategoryResponse>>> GetAllAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.ProductCategories.SearchAsync(query, cancellationToken);
        var responses = categories.Select(c => c.ToResponse()).ToList();
        return Result<IReadOnlyCollection<ProductCategoryResponse>>.Ok(responses, SuccessMessages.Catalog.GetAllProductCategories);
    }

    public async Task<Result<ProductCategoryResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return Result<ProductCategoryResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.NotFound);
        }

        return Result<ProductCategoryResponse>.Ok(category.ToResponse(), SuccessMessages.Catalog.GetProductCategory);
    }

    public async Task<Result<ProductCategoryResponse>> CreateAsync(CreateProductCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = request.ToEntity();

        await _unitOfWork.ProductCategories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductCategoryResponse>.Ok(category.ToResponse(), SuccessMessages.Catalog.CreateProductCategory, ResultStatus.Created);
    }

    public async Task<Result<ProductCategoryResponse>> UpdateAsync(Guid id, UpdateProductCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return Result<ProductCategoryResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.NotFound);
        }

        category.UpdateEntity(request);

        _unitOfWork.ProductCategories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductCategoryResponse>.Ok(category.ToResponse(), SuccessMessages.Catalog.UpdateProductCategory);
    }

    public async Task<Result<ProductCategoryResponse>> UploadImageAsync(Guid id, IFormFile imageFile, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return Result<ProductCategoryResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.NotFound);
        }

        string newImagePath;
        try
        {
            newImagePath = await _fileStorageService.SaveFileAsync(imageFile, "categories", cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result<ProductCategoryResponse>.Fail(ex.Message, ResultStatus.BadRequest);
        }
        catch
        {
            return Result<ProductCategoryResponse>.Fail(ErrorMessages.Catalog.ImageUploadFailed, ResultStatus.BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(category.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(category.ImagePath);
        }

        category.ImagePath = newImagePath;
        category.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ProductCategories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductCategoryResponse>.Ok(category.ToResponse(), SuccessMessages.Catalog.UploadProductCategoryImage);
    }

    public async Task<Result<ProductCategoryResponse>> DeleteImageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return Result<ProductCategoryResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(category.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(category.ImagePath);
            category.ImagePath = null;
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ProductCategories.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<ProductCategoryResponse>.Ok(category.ToResponse(), SuccessMessages.Catalog.DeleteProductCategoryImage);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.ProductCategories.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return Result.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(category.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(category.ImagePath);
        }

        _unitOfWork.ProductCategories.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Catalog.DeleteProductCategory);
    }
}
