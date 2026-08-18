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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomePal.Infrastructure.AI.CatalogManagement.Services;

public class ProductOfferScraperAgent : IProductOfferScraperService
{
    private readonly AIAgent _agent;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmbeddingService _embeddingService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IApifyScraperService _apifyScraperService;
    private readonly IScraperJobTracker _jobTracker;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductOfferScraperAgent> _logger;

    public ProductOfferScraperAgent(
        [FromKeyedServices("ProductScraperAgent")] AIAgent agent,
        IUnitOfWork unitOfWork,
        IEmbeddingService embeddingService,
        IFileStorageService fileStorageService,
        IApifyScraperService apifyScraperService,
        IScraperJobTracker jobTracker,
        IServiceScopeFactory serviceScopeFactory,
        HttpClient httpClient,
        ILogger<ProductOfferScraperAgent> logger)
    {
        _agent = agent;
        _unitOfWork = unitOfWork;
        _embeddingService = embeddingService;
        _fileStorageService = fileStorageService;
        _apifyScraperService = apifyScraperService;
        _jobTracker = jobTracker;
        _serviceScopeFactory = serviceScopeFactory;
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
        await ProcessImageBytesWithServicesAsync(_unitOfWork, _embeddingService, _fileStorageService, _logger, imageBytes, request.ImageFile.ContentType, request.SupermarketId, request.Caption, request.OcrText, request.SourceUrl, resultDto, cancellationToken);

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

        if (!_jobTracker.TryStartJob(request.SupermarketId))
        {
            return Result<OfferScraperResultDto>.Fail(ErrorMessages.Catalog.ScrapeJobInProgress, ResultStatus.Conflict);
        }

        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var scopedApify = scope.ServiceProvider.GetRequiredService<IApifyScraperService>();
            var scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var scopedEmbedding = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();
            var scopedStorage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
            var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<ProductOfferScraperAgent>>();
            var httpClientFactory = scope.ServiceProvider.GetService<IHttpClientFactory>();
            using var bgHttpClient = httpClientFactory != null ? httpClientFactory.CreateClient() : new HttpClient();

            try
            {
                var posts = await scopedApify.FetchFacebookPostsAsync(
                    request.PageUrl,
                    request.DaysBack,
                    request.ResultsLimit,
                    CancellationToken.None);

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
                            var imageBytes = await bgHttpClient.GetByteArrayAsync(media.ImgUrl, CancellationToken.None);
                            var prevOffers = resultDto.TotalExtractedOffers;

                            await ProcessImageBytesWithServicesAsync(
                                scopedUnitOfWork, scopedEmbedding, scopedStorage, scopedLogger,
                                imageBytes, "image/jpeg", request.SupermarketId, post.Text, media.OcrText, post.PostUrl, resultDto, CancellationToken.None);

                            var extractedInThisImage = resultDto.TotalExtractedOffers - prevOffers;
                            _jobTracker.UpdateProgress(1, extractedInThisImage);
                        }
                        catch (Exception ex)
                        {
                            scopedLogger.LogError(ex, "Failed to download/process media image from URL {ImgUrl}", media.ImgUrl);
                        }
                    }
                }

                _jobTracker.CompleteJob();
            }
            catch (Exception ex)
            {
                scopedLogger.LogError(ex, "Background Facebook scraping process failed for page {PageUrl}", request.PageUrl);
                _jobTracker.FailJob(ex.Message);
            }
        }, CancellationToken.None);

        var initialStatus = new OfferScraperResultDto();
        return Result<OfferScraperResultDto>.Ok(initialStatus, SuccessMessages.Catalog.ScrapeJobStarted, ResultStatus.Accepted);
    }

    private async Task ProcessImageBytesWithServicesAsync(
        IUnitOfWork unitOfWork,
        IEmbeddingService embeddingService,
        IFileStorageService fileStorageService,
        ILogger logger,
        byte[] imageBytes,
        string contentType,
        Guid supermarketId,
        string? caption,
        string? ocrText,
        string? sourceUrl,
        OfferScraperResultDto resultDto,
        CancellationToken cancellationToken)
    {
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var categories = await unitOfWork.ProductCategories.GetAllAsync(cancellationToken);
        var units = await unitOfWork.MeasuringUnits.GetAllAsync(cancellationToken);

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

            // 1. Crop image or fallback to full promotional image if box is unmapped/null
            string? croppedImagePath = null;
            var targetImageBytes = ImageCropperService.CropRegion(imageBytes, productRaw.BoundingBox) ?? imageBytes;
            if (targetImageBytes != null && targetImageBytes.Length > 0)
            {
                using var cropStream = new MemoryStream(targetImageBytes);
                var formFile = new FormFile(cropStream, 0, targetImageBytes.Length, "file", $"{Guid.NewGuid()}.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                };
                try
                {
                    croppedImagePath = await fileStorageService.SaveFileAsync(formFile, "products", cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to save product offer image.");
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
            var sqlVector = await embeddingService.GenerateSqlVectorAsync(textToEmbed, cancellationToken);

            // 4. Match Category & Unit IDs
            var matchedCategory = categories.FirstOrDefault(c => string.Equals(c.Name.Get("en"), productRaw.CategoryName, StringComparison.OrdinalIgnoreCase) || string.Equals(c.Name.Get("ar"), productRaw.CategoryName, StringComparison.OrdinalIgnoreCase));
            var matchedUnit = units.FirstOrDefault(u => string.Equals(u.Name.Get("en"), productRaw.UnitName, StringComparison.OrdinalIgnoreCase) || string.Equals(u.Name.Get("ar"), productRaw.UnitName, StringComparison.OrdinalIgnoreCase));

            Guid? categoryId = matchedCategory?.Id;
            Guid? unitId = matchedUnit?.Id;

            DateTime? validFromDate = ParseEgyptDateToUtc(productRaw.ValidFrom, isEndDate: false);
            DateTime? validToDate = ParseEgyptDateToUtc(productRaw.ValidTo, isEndDate: true);

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
                ValidFrom = validFromDate,
                ValidTo = validToDate,
                CategoryId = categoryId,
                ImagePath = croppedImagePath,
                SourceUrl = sourceUrl,
                SupermarketId = supermarketId,
                Embedding = sqlVector,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Offers.AddAsync(newOffer, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

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
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }
        public BoundingBoxDto? BoundingBox { get; set; }
    }

    private static DateTime? ParseEgyptDateToUtc(string? dateStr, bool isEndDate = false)
    {
        if (string.IsNullOrWhiteSpace(dateStr))
            return null;

        TimeZoneInfo egyptTzi;
        try
        {
            egyptTzi = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        }
        catch
        {
            try
            {
                egyptTzi = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");
            }
            catch
            {
                egyptTzi = TimeZoneInfo.Utc;
            }
        }

        string[] formats = ["yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy", "yyyy/MM/dd", "dd-MM-yyyy"];
        DateTime parsedDate;
        if (!DateTime.TryParseExact(dateStr.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            if (!DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return null;
            }
        }

        var localDateTime = isEndDate
            ? parsedDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)
            : parsedDate.Date;

        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, egyptTzi);
    }
}
