using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Pagination;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/products")]
public class CanonicalProductsController : BaseApiController
{
    private readonly ICanonicalProductService _productService;

    public CanonicalProductsController(ICanonicalProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get paginated list of canonical products with optional category filtering and search query
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<CanonicalProductResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCanonicalProducts([FromQuery] CanonicalProductQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.GetPagedAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Search canonical products by query term and optional category
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<CanonicalProductResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchCanonicalProducts([FromQuery] string? query, [FromQuery] Guid? categoryId, CancellationToken cancellationToken)
    {
        var request = new CanonicalProductQueryRequest
        {
            Query = query,
            CategoryId = categoryId
        };
        var result = await _productService.GetPagedAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single canonical product by ID
    /// </summary>
    [HttpGet("{canonicalProductId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<CanonicalProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCanonicalProductById(Guid canonicalProductId, CancellationToken cancellationToken)
    {
        var result = await _productService.GetByIdAsync(canonicalProductId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new canonical product (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<CanonicalProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCanonicalProduct([FromBody] CreateCanonicalProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.CreateAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing canonical product (Admin only)
    /// </summary>
    [HttpPut("{canonicalProductId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<CanonicalProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCanonicalProduct(Guid canonicalProductId, [FromBody] UpdateCanonicalProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.UpdateAsync(canonicalProductId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Upload cropped image for a canonical product (Admin only)
    /// </summary>
    [HttpPost("{canonicalProductId:guid}/image")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<CanonicalProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(Guid canonicalProductId, [FromForm] UploadProductImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.UploadImageAsync(canonicalProductId, request.Image, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete cropped image of a canonical product (Admin only)
    /// </summary>
    [HttpDelete("{canonicalProductId:guid}/image")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<CanonicalProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(Guid canonicalProductId, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteImageAsync(canonicalProductId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a canonical product (Admin only)
    /// </summary>
    [HttpDelete("{canonicalProductId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCanonicalProduct(Guid canonicalProductId, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteAsync(canonicalProductId, cancellationToken);
        return HandleResult(result);
    }
}
