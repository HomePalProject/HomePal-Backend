using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Pagination;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/offers")]
public class OffersController : BaseApiController
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    /// <summary>
    /// Get paginated list of offers with optional category, supermarket, and canonical product filtering
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<OfferResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOffers([FromQuery] OfferQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _offerService.GetPagedAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Search offers by query term and optional filters
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<OfferResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchOffers([FromQuery] string? query, [FromQuery] Guid? categoryId, [FromQuery] Guid? supermarketId, CancellationToken cancellationToken)
    {
        var request = new OfferQueryRequest
        {
            Query = query,
            CategoryId = categoryId,
            SupermarketId = supermarketId
        };
        var result = await _offerService.GetPagedAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Semantic vector search offers by meaning and similarity
    /// </summary>
    [HttpGet("semantic-search")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OfferResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SemanticSearchOffers([FromQuery] string query, [FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        var result = await _offerService.SearchOffersAsync(query, limit, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single offer by ID
    /// </summary>
    [HttpGet("{offerId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<OfferResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOfferById(Guid offerId, CancellationToken cancellationToken)
    {
        var result = await _offerService.GetByIdAsync(offerId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new offer (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<OfferResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateOffer([FromBody] CreateOfferRequest request, CancellationToken cancellationToken)
    {
        var result = await _offerService.CreateAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing offer (Admin only)
    /// </summary>
    [HttpPut("{offerId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<OfferResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOffer(Guid offerId, [FromBody] UpdateOfferRequest request, CancellationToken cancellationToken)
    {
        var result = await _offerService.UpdateAsync(offerId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Upload image for an offer (Admin only)
    /// </summary>
    [HttpPost("{offerId:guid}/image")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<OfferResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(Guid offerId, [FromForm] UploadOfferImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _offerService.UploadImageAsync(offerId, request.Image, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete image of an offer (Admin only)
    /// </summary>
    [HttpDelete("{offerId:guid}/image")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<OfferResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(Guid offerId, CancellationToken cancellationToken)
    {
        var result = await _offerService.DeleteImageAsync(offerId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete an offer (Admin only)
    /// </summary>
    [HttpDelete("{offerId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOffer(Guid offerId, CancellationToken cancellationToken)
    {
        var result = await _offerService.DeleteAsync(offerId, cancellationToken);
        return HandleResult(result);
    }
}
