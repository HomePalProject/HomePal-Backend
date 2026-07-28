using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IHouseholdInvitationService
{
    Task<Result<HouseholdInvitationResponse>> SendInvitationAsync(Guid managerUserId, SendInvitationRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<HouseholdInvitationResponse>>> GetMyInvitationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<HouseholdInvitationResponse>>> GetHouseholdInvitationsAsync(Guid managerUserId, CancellationToken cancellationToken = default);
    Task<Result> CancelInvitationAsync(Guid managerUserId, Guid invitationId, CancellationToken cancellationToken = default);
    Task<Result> AcceptInvitationAsync(Guid userId, Guid invitationId, CancellationToken cancellationToken = default);
    Task<Result> DeclineInvitationAsync(Guid userId, Guid invitationId, CancellationToken cancellationToken = default);
}
