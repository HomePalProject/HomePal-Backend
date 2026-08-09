using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/offers/scrape")]
public class ProductScraperController : BaseApiController
{
    private readonly IProductOfferScraperService _scraperService;

    public ProductScraperController(IProductOfferScraperService scraperService)
    {
        _scraperService = scraperService;
    }

    /// <summary>
    /// Scrape and process promotional offer images directly from a supermarket Facebook page URL via Apify API
    /// </summary>
    [HttpPost("facebook-page")]
    [Authorize(Roles = $"{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<OfferScraperResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScrapeFacebookPage([FromBody] ScrapeFacebookPageRequest request, CancellationToken cancellationToken)
    {
        var result = await _scraperService.ScrapeFacebookPageAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Extract product offers and create/match canonical products from an uploaded promotional offer image file
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
