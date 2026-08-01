using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/preferences/categories")]
public class PreferenceCategoriesController : BaseApiController


{
    private readonly IPreferenceCategoryService _categoryService;

    public PreferenceCategoriesController(IPreferenceCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all available preference categories
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PreferenceCategoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Search preference categories by query term
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PreferenceCategoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchCategories([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var result = await _categoryService.SearchCategoriesAsync(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get single preference category by ID
    /// </summary>
    [HttpGet("{categoryId:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PreferenceCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategoryByIdAsync(categoryId, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new preference category (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PreferenceCategoryResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCategory([FromBody] CreatePreferenceCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateCategoryAsync(CurrentUserId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing preference category (Admin only)
    /// </summary>
    [HttpPut("{categoryId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PreferenceCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdatePreferenceCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateCategoryAsync(CurrentUserId, categoryId, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a preference category (Admin only)
    /// </summary>
    [HttpDelete("{categoryId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteCategoryAsync(CurrentUserId, categoryId, cancellationToken);
        return HandleResult(result);
    }
}
