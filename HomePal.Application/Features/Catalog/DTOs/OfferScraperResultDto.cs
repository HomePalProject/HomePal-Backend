using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.DTOs;

public class ScrapeFacebookPageRequest
{
    [Required]
    public Guid SupermarketId { get; set; }

    [Required]
    [Url]
    public string PageUrl { get; set; } = string.Empty;

    public int DaysBack { get; set; } = 5;
    public int ResultsLimit { get; set; } = 20;
}

public class ProcessOfferImageRequest
{
    [Required]
    public Guid SupermarketId { get; set; }

    [Required]
    public IFormFile ImageFile { get; set; } = null!;

    public string? OcrText { get; set; }
    public string? Caption { get; set; }
    public string? SourceUrl { get; set; }
}

public class OfferScraperResultDto
{
    public List<OfferResponse> CreatedOffers { get; set; } = new();
    public int TotalScrapedImages { get; set; }
    public int TotalExtractedOffers { get; set; }
}
