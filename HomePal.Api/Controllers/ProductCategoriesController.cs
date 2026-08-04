using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/products/categories")]
public class ProductCategoriesController : BaseApiController
{
    private readonly IProductCategoryService _categoryService;

    public ProductCategoriesController(IProductCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all available product categories
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ProductCategoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllAsync(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Search product categories by query term
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<ProductCategoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchCategories([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllAsync(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single product category by ID
    /// </summary>
    [HttpGet("{categoryId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(categoryId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new product category (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateProductCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing product category (Admin only)
    /// </summary>
    [HttpPut("{categoryId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateProductCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateAsync(categoryId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Upload image for a product category (Admin only)
    /// </summary>
    [HttpPost("{categoryId:guid}/image")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(Guid categoryId, [FromForm] UploadCategoryImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UploadImageAsync(categoryId, request.Image, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete image of a product category (Admin only)
    /// </summary>
    [HttpDelete("{categoryId:guid}/image")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImage(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteImageAsync(categoryId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a product category (Admin only)
    /// </summary>
    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteAsync(categoryId, cancellationToken);
        return HandleResult(result);
    }
}
