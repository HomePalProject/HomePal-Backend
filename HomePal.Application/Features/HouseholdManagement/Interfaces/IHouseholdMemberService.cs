using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IHouseholdMemberService
{
    Task<Result<IReadOnlyCollection<HouseholdMemberResponse>>> GetHouseholdMembersAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<HouseholdMemberResponse>> GetMemberByIdAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken = default);
    Task<Result<HouseholdMemberResponse>> AddOfflineMemberAsync(Guid managerUserId, AddOfflineMemberRequest request, CancellationToken cancellationToken = default);
    Task<Result<HouseholdMemberResponse>> UpdateMemberAsync(Guid managerUserId, Guid memberId, UpdateMemberRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveMemberAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken = default);
}
