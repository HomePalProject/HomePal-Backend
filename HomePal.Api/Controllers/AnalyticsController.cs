using HomePal.Application.Features.Reports.DTOs;
using HomePal.Application.Features.Reports.Interfaces;
using HomePal.Domain.Constants;
using HomePal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomePal.Api.Controllers;

[Authorize(Roles = Roles.Admin)]
[Route("api/analytics")]
public class AnalyticsController : BaseApiController
{
    private readonly IAdminAnalyticsService _analyticsService;

    public AnalyticsController(IAdminAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// High-level metrics for stats (top chains, regional distribution, top categories)
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<AnalyticsOverviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetAnalyticsOverviewAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Regional demographic data and household density
    /// </summary>
    [HttpGet("demographics")]
    [ProducesResponseType(typeof(ApiResponse<GeographicDemographicsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDemographics(CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetGeographicDemographicsAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Aggregate statistics on household signups, active counts, and size distribution
    /// </summary>
    [HttpGet("households-summary")]
    [ProducesResponseType(typeof(ApiResponse<HouseholdsSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHouseholdsSummary(CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetHouseholdsSummaryAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Engagement signal for meal plan generation across households
    /// </summary>
    [HttpGet("meal-plans-summary")]
    [ProducesResponseType(typeof(ApiResponse<MealPlansSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMealPlansSummary(CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetMealPlansSummaryAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Product and category-level shopping behavior, top supermarket, and allergy ranking
    /// </summary>
    [HttpGet("shopping-trends")]
    [ProducesResponseType(typeof(ApiResponse<ShoppingTrendsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShoppingTrends(CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetShoppingTrendsAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Age and gender demographics for registered users and householders
    /// </summary>
    [HttpGet("user-demographics")]
    [ProducesResponseType(typeof(ApiResponse<UserDemographicsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserDemographics(CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetUserDemographicsAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// AI token usage and cost metrics aggregated via Langfuse API
    /// </summary>
    [HttpGet("tokens")]
    [HttpGet("token-usage")]
    [ProducesResponseType(typeof(ApiResponse<TokenUsageMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTokenMetrics([FromQuery] TokenMetricsFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetTokenMetricsAsync(filter, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Financial and subscription KPIs, revenue breakdown by plan, and monthly trend
    /// </summary>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(ApiResponse<RevenueAnalyticsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRevenue(CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetRevenueAnalyticsAsync(cancellationToken);
        return HandleResult(result);
    }
}
