using HomePal.Application.Features.Reports.DTOs;

namespace HomePal.Application.Features.Reports.Interfaces;

public interface IAdminAnalyticsRepository
{
    Task<AnalyticsOverviewDto> GetAnalyticsOverviewAsync(CancellationToken cancellationToken = default);
    Task<GeographicDemographicsDto> GetGeographicDemographicsAsync(CancellationToken cancellationToken = default);
    Task<HouseholdsSummaryDto> GetHouseholdsSummaryAsync(CancellationToken cancellationToken = default);
    Task<MealPlansSummaryDto> GetMealPlansSummaryAsync(CancellationToken cancellationToken = default);
    Task<ShoppingTrendsDto> GetShoppingTrendsAsync(CancellationToken cancellationToken = default);
    Task<UserDemographicsDto> GetUserDemographicsAsync(CancellationToken cancellationToken = default);
    Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync(CancellationToken cancellationToken = default);
}
