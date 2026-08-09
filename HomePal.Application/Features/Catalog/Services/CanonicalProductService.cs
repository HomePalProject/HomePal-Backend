using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.Catalog.Mappers;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.Services;

public class CanonicalProductService : ICanonicalProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IEmbeddingService _embeddingService;

    public CanonicalProductService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IEmbeddingService embeddingService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _embeddingService = embeddingService;
    }

    public async Task<Result<PaginatedList<CanonicalProductResponse>>> GetPagedAsync(CanonicalProductQueryRequest request, CancellationToken cancellationToken = default)
    {
        var pagedProducts = await _unitOfWork.CanonicalProducts.GetPagedAsync(
            request,
            request.Query,
            request.CategoryId,
            cancellationToken);

        var dtos = pagedProducts.Items.Select(p => p.ToResponse()).ToList();
        var result = PaginatedList<CanonicalProductResponse>.Create(
            dtos,
            pagedProducts.TotalCount,
            pagedProducts.PageNumber,
            request.PageSize);

        return Result<PaginatedList<CanonicalProductResponse>>.Ok(result, SuccessMessages.Catalog.GetCanonicalProducts);
    }

    public async Task<Result<CanonicalProductResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.CanonicalProducts.GetByIdWithDetailsAsync(id, cancellationToken);
        if (product == null)
        {
            return Result<CanonicalProductResponse>.Fail(ErrorMessages.Catalog.CanonicalProductNotFound, ResultStatus.NotFound);
        }

        return Result<CanonicalProductResponse>.Ok(product.ToResponse(), SuccessMessages.Catalog.GetCanonicalProduct);
    }

    public async Task<Result<CanonicalProductResponse>> CreateAsync(CreateCanonicalProductRequest request, CancellationToken cancellationToken = default)
    {
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _unitOfWork.ProductCategories.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (categoryExists == null)
            {
                return Result<CanonicalProductResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
            }
        }

        var product = request.ToEntity();

        var textToEmbed = string.Join(" ", product.Name.Select(n => n.Value)) + (product.Description != null ? " " + string.Join(" ", product.Description.Select(d => d.Value)) : "");
        product.Embedding = await _embeddingService.GenerateSqlVectorAsync(textToEmbed, cancellationToken);

        await _unitOfWork.CanonicalProducts.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdProduct = await _unitOfWork.CanonicalProducts.GetByIdWithDetailsAsync(product.Id, cancellationToken) ?? product;
        return Result<CanonicalProductResponse>.Ok(createdProduct.ToResponse(), SuccessMessages.Catalog.CreateCanonicalProduct, ResultStatus.Created);
    }

    public async Task<Result<CanonicalProductResponse>> UpdateAsync(Guid id, UpdateCanonicalProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.CanonicalProducts.GetByIdWithDetailsAsync(id, cancellationToken);
        if (product == null)
        {
            return Result<CanonicalProductResponse>.Fail(ErrorMessages.Catalog.CanonicalProductNotFound, ResultStatus.NotFound);
        }

        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _unitOfWork.ProductCategories.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (categoryExists == null)
            {
                return Result<CanonicalProductResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
            }
        }

        product.UpdateEntity(request);

        var textToEmbed = string.Join(" ", product.Name.Select(n => n.Value)) + (product.Description != null ? " " + string.Join(" ", product.Description.Select(d => d.Value)) : "");
        product.Embedding = await _embeddingService.GenerateSqlVectorAsync(textToEmbed, cancellationToken);

        _unitOfWork.CanonicalProducts.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProduct = await _unitOfWork.CanonicalProducts.GetByIdWithDetailsAsync(id, cancellationToken) ?? product;
        return Result<CanonicalProductResponse>.Ok(updatedProduct.ToResponse(), SuccessMessages.Catalog.UpdateCanonicalProduct);
    }

    public async Task<Result<CanonicalProductResponse>> UploadImageAsync(Guid id, IFormFile imageFile, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.CanonicalProducts.GetByIdWithDetailsAsync(id, cancellationToken);
        if (product == null)
        {
            return Result<CanonicalProductResponse>.Fail(ErrorMessages.Catalog.CanonicalProductNotFound, ResultStatus.NotFound);
        }

        string newImagePath;
        try
        {
            newImagePath = await _fileStorageService.SaveFileAsync(imageFile, "products", cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result<CanonicalProductResponse>.Fail(ex.Message, ResultStatus.BadRequest);
        }
        catch
        {
            return Result<CanonicalProductResponse>.Fail(ErrorMessages.Catalog.ImageUploadFailed, ResultStatus.BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(product.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(product.ImagePath);
        }

        product.ImagePath = newImagePath;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.CanonicalProducts.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProduct = await _unitOfWork.CanonicalProducts.GetByIdWithDetailsAsync(id, cancellationToken) ?? product;
        return Result<CanonicalProductResponse>.Ok(updatedProduct.ToResponse(), SuccessMessages.Catalog.UploadCanonicalProductImage);
    }

    public async Task<Result<CanonicalProductResponse>> DeleteImageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.CanonicalProducts.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return Result<CanonicalProductResponse>.Fail(ErrorMessages.Catalog.CanonicalProductNotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(product.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(product.ImagePath);
            product.ImagePath = null;
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CanonicalProducts.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var updatedProduct = await _unitOfWork.CanonicalProducts.GetByIdWithDetailsAsync(id, cancellationToken) ?? product;
        return Result<CanonicalProductResponse>.Ok(updatedProduct.ToResponse(), SuccessMessages.Catalog.DeleteCanonicalProductImage);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.CanonicalProducts.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return Result.Fail(ErrorMessages.Catalog.CanonicalProductNotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(product.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(product.ImagePath);
        }

        _unitOfWork.CanonicalProducts.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Catalog.DeleteCanonicalProduct);
    }
}
