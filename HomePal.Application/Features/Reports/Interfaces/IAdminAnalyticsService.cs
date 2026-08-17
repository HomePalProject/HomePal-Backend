using HomePal.Application.Features.Reports.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Reports.Interfaces;

public interface IAdminAnalyticsService
{
    Task<Result<AnalyticsOverviewDto>> GetAnalyticsOverviewAsync(CancellationToken cancellationToken = default);
    Task<Result<GeographicDemographicsDto>> GetGeographicDemographicsAsync(CancellationToken cancellationToken = default);
    Task<Result<HouseholdsSummaryDto>> GetHouseholdsSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<MealPlansSummaryDto>> GetMealPlansSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<ShoppingTrendsDto>> GetShoppingTrendsAsync(CancellationToken cancellationToken = default);
    Task<Result<UserDemographicsDto>> GetUserDemographicsAsync(CancellationToken cancellationToken = default);
    Task<Result<TokenUsageMetricsDto>> GetTokenMetricsAsync(TokenMetricsFilterDto? filter = null, CancellationToken cancellationToken = default);
    Task<Result<RevenueAnalyticsDto>> GetRevenueAnalyticsAsync(CancellationToken cancellationToken = default);
}
