using HomePal.Application.Features.Reports.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Reports.Interfaces;

public interface IHouseholdReportService
{
    Task<Result<HouseholdOverviewReportDto>> GetHouseholdOverviewAsync(Guid userId, CancellationToken cancellationToken = default);
}
