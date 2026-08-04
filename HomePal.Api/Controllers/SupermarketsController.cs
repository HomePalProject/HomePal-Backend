using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Pagination;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/supermarkets")]
public class SupermarketsController : BaseApiController
{
    private readonly ISupermarketService _supermarketService;

    public SupermarketsController(ISupermarketService supermarketService)
    {
        _supermarketService = supermarketService;
    }

    /// <summary>
    /// Get paginated list of supermarkets with optional search query
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<SupermarketResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSupermarkets([FromQuery] SupermarketQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _supermarketService.GetPagedAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Search supermarkets by query term
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<SupermarketResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchSupermarkets([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var request = new SupermarketQueryRequest { Query = query };
        var result = await _supermarketService.GetPagedAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single supermarket by ID
    /// </summary>
    [HttpGet("{supermarketId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<SupermarketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupermarketById(Guid supermarketId, CancellationToken cancellationToken)
    {
        var result = await _supermarketService.GetByIdAsync(supermarketId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new supermarket (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<SupermarketResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSupermarket([FromBody] CreateSupermarketRequest request, CancellationToken cancellationToken)
    {
        var result = await _supermarketService.CreateAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing supermarket (Admin only)
    /// </summary>
    [HttpPut("{supermarketId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<SupermarketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSupermarket(Guid supermarketId, [FromBody] UpdateSupermarketRequest request, CancellationToken cancellationToken)
    {
        var result = await _supermarketService.UpdateAsync(supermarketId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Upload logo image for a supermarket (Admin only)
    /// </summary>
    [HttpPost("{supermarketId:guid}/logo")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<SupermarketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadLogo(Guid supermarketId, [FromForm] UploadSupermarketLogoRequest request, CancellationToken cancellationToken)
    {
        var result = await _supermarketService.UploadLogoAsync(supermarketId, request.Image, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete logo of a supermarket (Admin only)
    /// </summary>
    [HttpDelete("{supermarketId:guid}/logo")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<SupermarketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLogo(Guid supermarketId, CancellationToken cancellationToken)
    {
        var result = await _supermarketService.DeleteLogoAsync(supermarketId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a supermarket (Admin only)
    /// </summary>
    [HttpDelete("{supermarketId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSupermarket(Guid supermarketId, CancellationToken cancellationToken)
    {
        var result = await _supermarketService.DeleteAsync(supermarketId, cancellationToken);
        return HandleResult(result);
    }
}
