using HomePal.Application.Features.Reports.DTOs;

namespace HomePal.Application.Features.Reports.Interfaces;

public interface IHouseholdReportRepository
{
    Task<HouseholdOverviewReportDto> GetHouseholdOverviewDataAsync(Guid householdId, CancellationToken cancellationToken = default);
}
