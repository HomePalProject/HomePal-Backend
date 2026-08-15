using HomePal.Application.Features.Reports.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Reports.Interfaces;

public interface ILangfuseMetricsService
{
    Task<Result<TokenUsageMetricsDto>> GetTokenMetricsAsync(TokenMetricsFilterDto? filter = null, CancellationToken cancellationToken = default);
}
