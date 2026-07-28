using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IHouseholdService
{
    Task<Result<HouseholdResponse>> CreateHouseholdAsync(Guid userId, CreateHouseholdRequest request, CancellationToken cancellationToken = default);
    Task<Result<HouseholdResponse>> GetMyHouseholdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<HouseholdResponse>> UpdateHouseholdAsync(Guid userId, UpdateHouseholdRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteHouseholdAsync(Guid userId, CancellationToken cancellationToken = default);
}
