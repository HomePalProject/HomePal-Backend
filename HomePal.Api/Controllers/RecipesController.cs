using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Pagination;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize]
[Route("api/recipes")]
public class RecipesController : BaseApiController
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    /// <summary>
    /// Get paginated list of recipes with comprehensive filtering options (search, difficulty, times, calories, preference & ingredient filters)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<RecipeResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecipes([FromQuery] RecipeFilterParams filter, CancellationToken cancellationToken)
    {
        var result = await _recipeService.GetRecipesAsync(filter, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get detailed recipe info by ID including full ingredient details and preferences
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.HouseholdMember},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<RecipeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecipeById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _recipeService.GetRecipeByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new recipe (Supports multipart/form-data for image upload along with recipe steps, ingredients & preferences)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<RecipeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRecipe([FromForm] CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        var result = await _recipeService.CreateRecipeAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update a recipe by ID (Supports multipart/form-data for image upload)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<RecipeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRecipe(Guid id, [FromForm] UpdateRecipeRequest request, CancellationToken cancellationToken)
    {
        var result = await _recipeService.UpdateRecipeAsync(id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a recipe by ID
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.HouseholdManager},{Roles.Admin}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRecipe(Guid id, CancellationToken cancellationToken)
    {
        var result = await _recipeService.DeleteRecipeAsync(id, cancellationToken);
        return HandleResult(result);
    }
}
