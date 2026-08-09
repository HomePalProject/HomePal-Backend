using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/offers/scrape")]
public class ProductScraperController : BaseApiController
{
    private readonly IProductOfferScraperService _scraperService;
    private readonly IScraperJobTracker _jobTracker;

    public ProductScraperController(
        IProductOfferScraperService scraperService,
        IScraperJobTracker jobTracker)
    {
        _scraperService = scraperService;
        _jobTracker = jobTracker;
    }

    /// <summary>
    /// Get current active or last completed scraping job status
    /// </summary>
    [HttpGet("status")]
    [Authorize(Roles = $"{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<ScraperJobStatusDto>), StatusCodes.Status200OK)]
    public IActionResult GetScrapeStatus()
    {
        var status = _jobTracker.GetStatus();
        return HandleResult(Result<ScraperJobStatusDto>.Ok(status));
    }

    /// <summary>
    /// Trigger background Apify Facebook page scraping (Runs in background, enforces single-job concurrency lock, returns 202 Accepted)
    /// </summary>
    [HttpPost("facebook-page")]
    [Authorize(Roles = $"{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<OfferScraperResultDto>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScrapeFacebookPage([FromBody] ScrapeFacebookPageRequest request, CancellationToken cancellationToken)
    {
        var result = await _scraperService.ScrapeFacebookPageAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Extract product offers from an uploaded promotional offer image file
    /// </summary>
    [HttpPost("image-file")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = $"{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<OfferScraperResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScrapeImageFile([FromForm] ProcessOfferImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _scraperService.ProcessImageAsync(request, cancellationToken);
        return HandleResult(result);
    }
}
