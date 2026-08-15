using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Reports.DTOs;
using HomePal.Application.Features.Reports.Interfaces;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Reports.Services;

public class AdminAnalyticsService : IAdminAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILangfuseMetricsService _langfuseMetricsService;

    public AdminAnalyticsService(
        IUnitOfWork unitOfWork,
        ILangfuseMetricsService langfuseMetricsService)
    {
        _unitOfWork = unitOfWork;
        _langfuseMetricsService = langfuseMetricsService;
    }

    public async Task<Result<AnalyticsOverviewDto>> GetAnalyticsOverviewAsync(CancellationToken cancellationToken = default)
    {
        var data = await _unitOfWork.AdminAnalytics.GetAnalyticsOverviewAsync(cancellationToken);
        return Result<AnalyticsOverviewDto>.Ok(data, "Analytics.FetchSuccess");
    }

    public async Task<Result<GeographicDemographicsDto>> GetGeographicDemographicsAsync(CancellationToken cancellationToken = default)
    {
        var data = await _unitOfWork.AdminAnalytics.GetGeographicDemographicsAsync(cancellationToken);
        return Result<GeographicDemographicsDto>.Ok(data, "Analytics.FetchSuccess");
    }

    public async Task<Result<HouseholdsSummaryDto>> GetHouseholdsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var data = await _unitOfWork.AdminAnalytics.GetHouseholdsSummaryAsync(cancellationToken);
        return Result<HouseholdsSummaryDto>.Ok(data, "Analytics.FetchSuccess");
    }

    public async Task<Result<MealPlansSummaryDto>> GetMealPlansSummaryAsync(CancellationToken cancellationToken = default)
    {
        var data = await _unitOfWork.AdminAnalytics.GetMealPlansSummaryAsync(cancellationToken);
        return Result<MealPlansSummaryDto>.Ok(data, "Analytics.FetchSuccess");
    }

    public async Task<Result<ShoppingTrendsDto>> GetShoppingTrendsAsync(CancellationToken cancellationToken = default)
    {
        var data = await _unitOfWork.AdminAnalytics.GetShoppingTrendsAsync(cancellationToken);
        return Result<ShoppingTrendsDto>.Ok(data, "Analytics.FetchSuccess");
    }

    public async Task<Result<UserDemographicsDto>> GetUserDemographicsAsync(CancellationToken cancellationToken = default)
    {
        var data = await _unitOfWork.AdminAnalytics.GetUserDemographicsAsync(cancellationToken);
        return Result<UserDemographicsDto>.Ok(data, "Analytics.FetchSuccess");
    }

    public async Task<Result<TokenUsageMetricsDto>> GetTokenMetricsAsync(TokenMetricsFilterDto? filter = null, CancellationToken cancellationToken = default)
    {
        return await _langfuseMetricsService.GetTokenMetricsAsync(filter, cancellationToken);
    }
}
