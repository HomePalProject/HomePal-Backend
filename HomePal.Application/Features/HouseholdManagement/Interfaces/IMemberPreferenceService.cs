using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IMemberPreferenceService
{
    Task<Result<IReadOnlyCollection<PreferenceResponse>>> GetMemberPreferencesAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PreferenceResponse>>> SetMemberPreferencesAsync(Guid currentUserId, Guid memberId, AssignPreferencesRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveMemberPreferenceAsync(Guid currentUserId, Guid memberId, Guid preferenceId, CancellationToken cancellationToken = default);
}
