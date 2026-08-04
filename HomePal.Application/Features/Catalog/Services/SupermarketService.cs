using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.Catalog.Mappers;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.Services;

public class SupermarketService : ISupermarketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public SupermarketService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<IReadOnlyCollection<SupermarketResponse>>> GetAllAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        var supermarkets = await _unitOfWork.Supermarkets.SearchAsync(query, cancellationToken);
        var responses = supermarkets.Select(s => s.ToResponse()).ToList();
        return Result<IReadOnlyCollection<SupermarketResponse>>.Ok(responses, SuccessMessages.Catalog.GetAllSupermarkets);
    }

    public async Task<Result<PaginatedList<SupermarketResponse>>> GetPagedAsync(SupermarketQueryRequest request, CancellationToken cancellationToken = default)
    {
        var pagedSupermarkets = await _unitOfWork.Supermarkets.GetPagedAsync(
            request,
            request.Query,
            cancellationToken);

        var dtos = pagedSupermarkets.Items.Select(s => s.ToResponse()).ToList();
        var result = PaginatedList<SupermarketResponse>.Create(
            dtos,
            pagedSupermarkets.TotalCount,
            pagedSupermarkets.PageNumber,
            request.PageSize);

        return Result<PaginatedList<SupermarketResponse>>.Ok(result, SuccessMessages.Catalog.GetAllSupermarkets);
    }

    public async Task<Result<SupermarketResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(id, cancellationToken);
        if (supermarket == null)
        {
            return Result<SupermarketResponse>.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.NotFound);
        }

        return Result<SupermarketResponse>.Ok(supermarket.ToResponse(), SuccessMessages.Catalog.GetSupermarket);
    }

    public async Task<Result<SupermarketResponse>> CreateAsync(CreateSupermarketRequest request, CancellationToken cancellationToken = default)
    {
        var supermarket = request.ToEntity();

        await _unitOfWork.Supermarkets.AddAsync(supermarket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SupermarketResponse>.Ok(supermarket.ToResponse(), SuccessMessages.Catalog.CreateSupermarket, ResultStatus.Created);
    }

    public async Task<Result<SupermarketResponse>> UpdateAsync(Guid id, UpdateSupermarketRequest request, CancellationToken cancellationToken = default)
    {
        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(id, cancellationToken);
        if (supermarket == null)
        {
            return Result<SupermarketResponse>.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.NotFound);
        }

        supermarket.UpdateEntity(request);

        _unitOfWork.Supermarkets.Update(supermarket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SupermarketResponse>.Ok(supermarket.ToResponse(), SuccessMessages.Catalog.UpdateSupermarket);
    }

    public async Task<Result<SupermarketResponse>> UploadLogoAsync(Guid id, IFormFile imageFile, CancellationToken cancellationToken = default)
    {
        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(id, cancellationToken);
        if (supermarket == null)
        {
            return Result<SupermarketResponse>.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.NotFound);
        }

        string newLogoPath;
        try
        {
            newLogoPath = await _fileStorageService.SaveFileAsync(imageFile, "supermarkets", cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result<SupermarketResponse>.Fail(ex.Message, ResultStatus.BadRequest);
        }
        catch
        {
            return Result<SupermarketResponse>.Fail(ErrorMessages.Catalog.ImageUploadFailed, ResultStatus.BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(supermarket.LogoPath))
        {
            await _fileStorageService.DeleteFileAsync(supermarket.LogoPath);
        }

        supermarket.LogoPath = newLogoPath;
        supermarket.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Supermarkets.Update(supermarket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SupermarketResponse>.Ok(supermarket.ToResponse(), SuccessMessages.Catalog.UploadSupermarketLogo);
    }

    public async Task<Result<SupermarketResponse>> DeleteLogoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(id, cancellationToken);
        if (supermarket == null)
        {
            return Result<SupermarketResponse>.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(supermarket.LogoPath))
        {
            await _fileStorageService.DeleteFileAsync(supermarket.LogoPath);
            supermarket.LogoPath = null;
            supermarket.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Supermarkets.Update(supermarket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<SupermarketResponse>.Ok(supermarket.ToResponse(), SuccessMessages.Catalog.DeleteSupermarketLogo);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(id, cancellationToken);
        if (supermarket == null)
        {
            return Result.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(supermarket.LogoPath))
        {
            await _fileStorageService.DeleteFileAsync(supermarket.LogoPath);
        }

        _unitOfWork.Supermarkets.Remove(supermarket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Catalog.DeleteSupermarket);
    }
}
