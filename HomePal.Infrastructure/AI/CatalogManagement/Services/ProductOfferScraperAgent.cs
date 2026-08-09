using System.Globalization;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.Catalog.Mappers;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;
using HomePal.Infrastructure.AI.CatalogManagement.Instructions;
using HomePal.Infrastructure.AI.Services;
using HomePal.Shared.Results;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace HomePal.Infrastructure.AI.CatalogManagement.Services;

public class ProductOfferScraperAgent : IProductOfferScraperService
{
    private readonly AIAgent _agent;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmbeddingService _embeddingService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IApifyScraperService _apifyScraperService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductOfferScraperAgent> _logger;

    public ProductOfferScraperAgent(
        AIAgent agent,
        IUnitOfWork unitOfWork,
        IEmbeddingService embeddingService,
        IFileStorageService fileStorageService,
        IApifyScraperService apifyScraperService,
        HttpClient httpClient,
        ILogger<ProductOfferScraperAgent> logger)
    {
        _agent = agent;
        _unitOfWork = unitOfWork;
        _embeddingService = embeddingService;
        _fileStorageService = fileStorageService;
        _apifyScraperService = apifyScraperService;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<OfferScraperResultDto>> ProcessImageAsync(
        ProcessOfferImageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ImageFile == null || request.ImageFile.Length == 0)
        {
            return Result<OfferScraperResultDto>.Fail(ErrorMessages.Auth.InvalidImageFile, ResultStatus.BadRequest);
        }

        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(request.SupermarketId, cancellationToken);
        if (supermarket == null)
        {
            return Result<OfferScraperResultDto>.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.BadRequest);
        }

        using var memoryStream = new MemoryStream();
        await request.ImageFile.CopyToAsync(memoryStream, cancellationToken);
        var imageBytes = memoryStream.ToArray();

        var resultDto = new OfferScraperResultDto { TotalScrapedImages = 1 };
        await ProcessImageBytesAsync(imageBytes, request.ImageFile.ContentType, request.SupermarketId, request.Caption, request.OcrText, resultDto, cancellationToken);

        return Result<OfferScraperResultDto>.Ok(resultDto, SuccessMessages.Catalog.GetOffers);
    }

    public async Task<Result<OfferScraperResultDto>> ScrapeFacebookPageAsync(
        ScrapeFacebookPageRequest request,
        CancellationToken cancellationToken = default)
    {
        var supermarket = await _unitOfWork.Supermarkets.GetByIdAsync(request.SupermarketId, cancellationToken);
        if (supermarket == null)
        {
            return Result<OfferScraperResultDto>.Fail(ErrorMessages.Catalog.SupermarketNotFound, ResultStatus.BadRequest);
        }

        var posts = await _apifyScraperService.FetchFacebookPostsAsync(
            request.PageUrl,
            request.DaysBack,
            request.ResultsLimit,
            cancellationToken);

        var resultDto = new OfferScraperResultDto();

        foreach (var post in posts)
        {
            if (post.Media == null || post.Media.Count == 0)
                continue;

            foreach (var media in post.Media)
            {
                if (string.IsNullOrWhiteSpace(media.ImgUrl))
                    continue;

                try
                {
                    var imageBytes = await _httpClient.GetByteArrayAsync(media.ImgUrl, cancellationToken);
                    resultDto.TotalScrapedImages++;
                    await ProcessImageBytesAsync(imageBytes, "image/jpeg", request.SupermarketId, post.Text, media.OcrText, resultDto, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to download/process media image from URL {ImgUrl}", media.ImgUrl);
                }
            }
        }

        return Result<OfferScraperResultDto>.Ok(resultDto, SuccessMessages.Catalog.GetOffers);
    }

    private async Task ProcessImageBytesAsync(
        byte[] imageBytes,
        string contentType,
        Guid supermarketId,
        string? caption,
        string? ocrText,
        OfferScraperResultDto resultDto,
        CancellationToken cancellationToken)
    {
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var categories = await _unitOfWork.ProductCategories.GetAllAsync(cancellationToken);
        var units = await _unitOfWork.MeasuringUnits.GetAllAsync(cancellationToken);

        var categoriesFormatted = string.Join("\n", categories.Select(c => $"- ID: {c.Id}, Name: {c.Name.Get(culture)}"));
        var unitsFormatted = string.Join("\n", units.Select(u => $"- ID: {u.Id}, Name: {u.Name.Get(culture)}"));

        var promptText = ProductScraperInstructions.BuildPrompt(categoriesFormatted, unitsFormatted, culture);
        if (!string.IsNullOrWhiteSpace(caption) || !string.IsNullOrWhiteSpace(ocrText))
        {
            promptText += $"\n\nContext - Caption: {caption}\nContext - Image OCR Text: {ocrText}";
        }

        var mediaType = string.IsNullOrWhiteSpace(contentType) ? "image/jpeg" : contentType;
        var userMessage = new ChatMessage(ChatRole.User, [
            new TextContent(promptText),
            new DataContent(imageBytes, mediaType)
        ]);

        var agentResponse = await _agent.RunAsync<ProductScraperRawResult>(userMessage, cancellationToken: cancellationToken);
        var rawResult = agentResponse.Result;

        if (rawResult?.Products == null || rawResult.Products.Count == 0)
            return;

        foreach (var productRaw in rawResult.Products)
        {
            if (string.IsNullOrWhiteSpace(productRaw.Name))
                continue;

            resultDto.TotalExtractedOffers++;

            // 1. Crop image
            string? croppedImagePath = null;
            var croppedBytes = ImageCropperService.CropRegion(imageBytes, productRaw.BoundingBox);
            if (croppedBytes != null && croppedBytes.Length > 0)
            {
                using var cropStream = new MemoryStream(croppedBytes);
                var formFile = new FormFile(cropStream, 0, croppedBytes.Length, "file", $"{Guid.NewGuid()}.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                };
                try
                {
                    croppedImagePath = await _fileStorageService.SaveFileAsync(formFile, "products", cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save cropped product image.");
                }
            }

            // 2. Localized Name & Description
            var localizedNames = new List<LocalizedItem>
            {
                new LocalizedItem("en", productRaw.Name)
            };
            if (!string.IsNullOrWhiteSpace(productRaw.NameAr))
            {
                localizedNames.Add(new LocalizedItem("ar", productRaw.NameAr));
            }

            var localizedDescriptions = new List<LocalizedItem>();
            if (!string.IsNullOrWhiteSpace(productRaw.Description))
            {
                localizedDescriptions.Add(new LocalizedItem("en", productRaw.Description));
            }
            if (!string.IsNullOrWhiteSpace(productRaw.DescriptionAr))
            {
                localizedDescriptions.Add(new LocalizedItem("ar", productRaw.DescriptionAr));
            }

            // 3. Vector Embedding
            var textToEmbed = $"{productRaw.Name} {productRaw.NameAr} {productRaw.Description} {productRaw.DescriptionAr}".Trim();
            var sqlVector = await _embeddingService.GenerateSqlVectorAsync(textToEmbed, cancellationToken);

            // 4. Match Category & Unit IDs
            var matchedCategory = categories.FirstOrDefault(c => string.Equals(c.Name.Get("en"), productRaw.CategoryName, StringComparison.OrdinalIgnoreCase) || string.Equals(c.Name.Get("ar"), productRaw.CategoryName, StringComparison.OrdinalIgnoreCase));
            var matchedUnit = units.FirstOrDefault(u => string.Equals(u.Name.Get("en"), productRaw.UnitName, StringComparison.OrdinalIgnoreCase) || string.Equals(u.Name.Get("ar"), productRaw.UnitName, StringComparison.OrdinalIgnoreCase));

            Guid? categoryId = matchedCategory?.Id;
            Guid? unitId = matchedUnit?.Id;

            // 5. Create Standalone Offer
            var newOffer = new Offer
            {
                Id = Guid.NewGuid(),
                Name = localizedNames,
                Description = localizedDescriptions.Count > 0 ? localizedDescriptions : null,
                Quantity = (double)(productRaw.Quantity > 0 ? productRaw.Quantity : 1),
                UnitId = unitId,
                OriginalPrice = productRaw.OriginalPrice ?? productRaw.DiscountedPrice ?? 0,
                DiscountedPrice = productRaw.DiscountedPrice ?? productRaw.OriginalPrice ?? 0,
                CategoryId = categoryId,
                ImagePath = croppedImagePath,
                SupermarketId = supermarketId,
                Embedding = sqlVector,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Offers.AddAsync(newOffer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            resultDto.CreatedOffers.Add(newOffer.ToResponse(culture));
        }
    }

    private class ProductScraperRawResult
    {
        public List<ProductScraperRawItem>? Products { get; set; }
    }

    private class ProductScraperRawItem
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitName { get; set; }
        public string? CategoryName { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public BoundingBoxDto? BoundingBox { get; set; }
    }
}
