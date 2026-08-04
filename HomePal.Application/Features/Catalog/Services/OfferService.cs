using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.Catalog.Mappers;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.Services;

public class OfferService : IOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public OfferService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<PaginatedList<OfferResponse>>> GetPagedAsync(OfferQueryRequest request, CancellationToken cancellationToken = default)
    {
        var pagedOffers = await _unitOfWork.Offers.GetPagedAsync(
            request,
            request.Query,
            request.CategoryId,
            request.SupermarketId,
            request.CanonicalProductId,
            cancellationToken);

        var dtos = pagedOffers.Items.Select(o => o.ToResponse()).ToList();
        var result = PaginatedList<OfferResponse>.Create(
            dtos,
            pagedOffers.TotalCount,
            pagedOffers.PageNumber,
            request.PageSize);

        return Result<PaginatedList<OfferResponse>>.Ok(result, SuccessMessages.Catalog.GetOffers);
    }

    public async Task<Result<OfferResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(id, cancellationToken);
        if (offer == null)
        {
            return Result<OfferResponse>.Fail(ErrorMessages.Catalog.OfferNotFound, ResultStatus.NotFound);
        }

        return Result<OfferResponse>.Ok(offer.ToResponse(), SuccessMessages.Catalog.GetOffer);
    }

    public async Task<Result<OfferResponse>> CreateAsync(CreateOfferRequest request, CancellationToken cancellationToken = default)
    {
        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(request.SupermarketId, cancellationToken);
        if (supermarket == null)
        {
            return Result<OfferResponse>.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.BadRequest);
        }

        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                return Result<OfferResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
            }
        }

        if (request.UnitId.HasValue)
        {
            var unit = await _unitOfWork.MeasuringUnits.GetByIdAsync(request.UnitId.Value, cancellationToken);
            if (unit == null)
            {
                return Result<OfferResponse>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.BadRequest);
            }
        }

        if (request.CanonicalProductId.HasValue)
        {
            var product = await _unitOfWork.CanonicalProducts.GetByIdAsync(request.CanonicalProductId.Value, cancellationToken);
            if (product == null)
            {
                return Result<OfferResponse>.Fail(ErrorMessages.Catalog.CanonicalProductNotFound, ResultStatus.BadRequest);
            }
        }

        var offer = request.ToEntity();

        await _unitOfWork.Offers.AddAsync(offer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdOffer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(offer.Id, cancellationToken) ?? offer;
        return Result<OfferResponse>.Ok(createdOffer.ToResponse(), SuccessMessages.Catalog.CreateOffer, ResultStatus.Created);
    }

    public async Task<Result<OfferResponse>> UpdateAsync(Guid id, UpdateOfferRequest request, CancellationToken cancellationToken = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(id, cancellationToken);
        if (offer == null)
        {
            return Result<OfferResponse>.Fail(ErrorMessages.Catalog.OfferNotFound, ResultStatus.NotFound);
        }

        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(request.SupermarketId, cancellationToken);
        if (supermarket == null)
        {
            return Result<OfferResponse>.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.BadRequest);
        }

        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                return Result<OfferResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
            }
        }

        if (request.UnitId.HasValue)
        {
            var unit = await _unitOfWork.MeasuringUnits.GetByIdAsync(request.UnitId.Value, cancellationToken);
            if (unit == null)
            {
                return Result<OfferResponse>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.BadRequest);
            }
        }

        if (request.CanonicalProductId.HasValue)
        {
            var product = await _unitOfWork.CanonicalProducts.GetByIdAsync(request.CanonicalProductId.Value, cancellationToken);
            if (product == null)
            {
                return Result<OfferResponse>.Fail(ErrorMessages.Catalog.CanonicalProductNotFound, ResultStatus.BadRequest);
            }
        }

        offer.UpdateEntity(request);

        _unitOfWork.Offers.Update(offer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedOffer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(id, cancellationToken) ?? offer;
        return Result<OfferResponse>.Ok(updatedOffer.ToResponse(), SuccessMessages.Catalog.UpdateOffer);
    }

    public async Task<Result<OfferResponse>> UploadImageAsync(Guid id, IFormFile imageFile, CancellationToken cancellationToken = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(id, cancellationToken);
        if (offer == null)
        {
            return Result<OfferResponse>.Fail(ErrorMessages.Catalog.OfferNotFound, ResultStatus.NotFound);
        }

        string newImagePath;
        try
        {
            newImagePath = await _fileStorageService.SaveFileAsync(imageFile, "offers", cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result<OfferResponse>.Fail(ex.Message, ResultStatus.BadRequest);
        }
        catch
        {
            return Result<OfferResponse>.Fail(ErrorMessages.Catalog.ImageUploadFailed, ResultStatus.BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(offer.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(offer.ImagePath);
        }

        offer.ImagePath = newImagePath;
        offer.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Offers.Update(offer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedOffer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(id, cancellationToken) ?? offer;
        return Result<OfferResponse>.Ok(updatedOffer.ToResponse(), SuccessMessages.Catalog.UploadOfferImage);
    }

    public async Task<Result<OfferResponse>> DeleteImageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(id, cancellationToken);
        if (offer == null)
        {
            return Result<OfferResponse>.Fail(ErrorMessages.Catalog.OfferNotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(offer.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(offer.ImagePath);
            offer.ImagePath = null;
            offer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Offers.Update(offer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var updatedOffer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(id, cancellationToken) ?? offer;
        return Result<OfferResponse>.Ok(updatedOffer.ToResponse(), SuccessMessages.Catalog.DeleteOfferImage);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(id, cancellationToken);
        if (offer == null)
        {
            return Result.Fail(ErrorMessages.Catalog.OfferNotFound, ResultStatus.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(offer.ImagePath))
        {
            await _fileStorageService.DeleteFileAsync(offer.ImagePath);
        }

        _unitOfWork.Offers.Remove(offer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Catalog.DeleteOffer);
    }
}
