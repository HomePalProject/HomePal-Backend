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
    private readonly IEmbeddingService _embeddingService;

    public OfferService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IEmbeddingService embeddingService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _embeddingService = embeddingService;
    }

    public async Task<Result<PaginatedList<OfferResponse>>> GetPagedAsync(OfferQueryRequest request, CancellationToken cancellationToken = default)
    {
        var pagedOffers = await _unitOfWork.Offers.GetPagedAsync(
            request,
            request.Query,
            request.CategoryId,
            request.SupermarketId,
            request.OnlyVerified,
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

        var offer = request.ToEntity();

        var textToEmbed = string.Join(" ", offer.Name.Select(n => n.Value)) + (offer.Description != null ? " " + string.Join(" ", offer.Description.Select(d => d.Value)) : "");
        offer.Embedding = await _embeddingService.GenerateSqlVectorAsync(textToEmbed, cancellationToken);

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

        offer.UpdateEntity(request);

        var textToEmbed = string.Join(" ", offer.Name.Select(n => n.Value)) + (offer.Description != null ? " " + string.Join(" ", offer.Description.Select(d => d.Value)) : "");
        offer.Embedding = await _embeddingService.GenerateSqlVectorAsync(textToEmbed, cancellationToken);

        _unitOfWork.Offers.Update(offer);
        await _unitOfWork.ShoppingListItems.UpdateFromOfferAsync(offer, cancellationToken);
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

    public async Task<Result<IReadOnlyList<OfferResponse>>> SearchOffersAsync(string query, int limit = 10, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Result<IReadOnlyList<OfferResponse>>.Ok(Array.Empty<OfferResponse>(), SuccessMessages.Catalog.GetOffers);
        }

        var clampedLimit = Math.Clamp(limit, 1, 50);
        var queryEmbedding = await _embeddingService.GenerateSqlVectorAsync(query, cancellationToken);

        var offers = await _unitOfWork.Offers.SearchSemanticAsync(
            queryEmbedding,
            take: clampedLimit,
            cancellationToken: cancellationToken);

        var dtos = offers.Select(o => o.ToResponse()).ToList();
        return Result<IReadOnlyList<OfferResponse>>.Ok(dtos, SuccessMessages.Catalog.GetOffers);
    }
}
