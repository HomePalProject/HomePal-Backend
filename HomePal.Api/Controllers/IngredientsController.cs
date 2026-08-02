using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Pagination;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/ingredients")]
public class IngredientsController : BaseApiController
{
    private readonly IIngredientService _ingredientService;

    public IngredientsController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    /// <summary>
    /// Get paginated list of ingredients with optional search query
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<IngredientResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIngredients([FromQuery] PaginationRequest request, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await _ingredientService.GetIngredientsAsync(request, search, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get ingredient details by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IngredientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIngredientById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _ingredientService.GetIngredientByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new ingredient (Supports multipart/form-data for optional picture upload)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IngredientResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIngredient([FromForm] CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        var result = await _ingredientService.CreateIngredientAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an ingredient by ID (Supports multipart/form-data for optional picture upload)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<IngredientResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIngredient(Guid id, [FromForm] UpdateIngredientRequest request, CancellationToken cancellationToken)
    {
        var result = await _ingredientService.UpdateIngredientAsync(id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete an ingredient by ID
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIngredient(Guid id, CancellationToken cancellationToken)
    {
        var result = await _ingredientService.DeleteIngredientAsync(id, cancellationToken);
        return HandleResult(result);
    }
}
