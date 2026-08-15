using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Reports.DTOs;
using HomePal.Application.Features.Reports.Interfaces;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Reports.Services;

public class HouseholdReportService : IHouseholdReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public HouseholdReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HouseholdOverviewReportDto>> GetHouseholdOverviewAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var member = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        if (member == null)
        {
            return Result<HouseholdOverviewReportDto>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        var report = await _unitOfWork.Reports.GetHouseholdOverviewDataAsync(member.HouseholdId, cancellationToken);
        return Result<HouseholdOverviewReportDto>.Ok(report, SuccessMessages.Reports.Fetch);
    }
}
