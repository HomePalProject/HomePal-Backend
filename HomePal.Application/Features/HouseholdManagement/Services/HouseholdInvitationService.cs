using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Mappers;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Domain.Enums;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Identity;

namespace HomePal.Application.Features.HouseholdManagement.Services;

public class HouseholdInvitationService : IHouseholdInvitationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IUnitOfWork _unitOfWork;

    public HouseholdInvitationService(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HouseholdInvitationResponse>> SendInvitationAsync(Guid managerUserId, SendInvitationRequest request, CancellationToken cancellationToken = default)
    {
        var managerMember = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(managerUserId, cancellationToken);
        if (managerMember == null || managerMember.Household == null)
        {
            return Result<HouseholdInvitationResponse>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        if (managerMember.Role != Roles.HouseholdManager)
        {
            return Result<HouseholdInvitationResponse>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var input = request.InvitedUserNameOrEmail.Trim();
        var targetUser = await _userManager.FindByNameAsync(input) 
                         ?? await _userManager.FindByEmailAsync(input);

        if (targetUser != null)
        {
            var isAlreadyInHousehold = await _unitOfWork.HouseholdMembers.IsUserInAnyHouseholdAsync(targetUser.Id, cancellationToken);
            if (isAlreadyInHousehold)
            {
                return Result<HouseholdInvitationResponse>.Fail(ErrorMessages.Household.UserAlreadyInHousehold, ResultStatus.BadRequest);
            }
        }

        var existingPendingInvite = await _unitOfWork.HouseholdInvitations.HasPendingInvitationAsync(managerMember.HouseholdId, input, cancellationToken);
        if (existingPendingInvite)
        {
            return Result<HouseholdInvitationResponse>.Fail(ErrorMessages.Household.PendingInvitationExists, ResultStatus.BadRequest);
        }

        var inviterUser = await _userManager.FindByIdAsync(managerUserId.ToString());
        var recipientEmail = targetUser?.Email ?? (input.Contains('@') ? input : null);

        var invitation = new HouseholdInvitation
        {
            Id = Guid.NewGuid(),
            HouseholdId = managerMember.HouseholdId,
            InvitedEmail = recipientEmail,
            InvitedUserName = targetUser?.UserName ?? (!input.Contains('@') ? input : null),
            InvitedById = managerUserId,
            Status = InvitationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.HouseholdInvitations.AddAsync(invitation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(recipientEmail))
        {
            try
            {
                await _emailSender.SendInvitationEmailAsync(
                    recipientEmail,
                    inviterUser?.FullName ?? "Household Manager",
                    cancellationToken);
            }
            catch
            {
                // Email sending failure shouldn't abort invitation record creation
            }
        }

        return Result<HouseholdInvitationResponse>.Ok(
            invitation.ToResponse(managerMember.Household.Name, inviterUser?.FullName ?? string.Empty),
            SuccessMessages.Household.SendInvitation,
            ResultStatus.Created);
    }

    public async Task<Result<IReadOnlyCollection<HouseholdInvitationResponse>>> GetMyInvitationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<IReadOnlyCollection<HouseholdInvitationResponse>>.Fail(ErrorMessages.Household.UserNotFound, ResultStatus.NotFound);
        }

        var invitations = await _unitOfWork.HouseholdInvitations.GetPendingByEmailOrUsernameAsync(user.Email, user.UserName, cancellationToken);

        return Result<IReadOnlyCollection<HouseholdInvitationResponse>>.Ok(invitations.ToResponseList(), SuccessMessages.Household.GetInvitations);
    }

    public async Task<Result<IReadOnlyCollection<HouseholdInvitationResponse>>> GetHouseholdInvitationsAsync(Guid managerUserId, CancellationToken cancellationToken = default)
    {
        var managerMember = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(managerUserId, cancellationToken);
        if (managerMember == null)
        {
            return Result<IReadOnlyCollection<HouseholdInvitationResponse>>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        if (managerMember.Role != Roles.HouseholdManager)
        {
            return Result<IReadOnlyCollection<HouseholdInvitationResponse>>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var invitations = await _unitOfWork.HouseholdInvitations.GetByHouseholdIdAsync(managerMember.HouseholdId, cancellationToken);

        return Result<IReadOnlyCollection<HouseholdInvitationResponse>>.Ok(invitations.ToResponseList(), SuccessMessages.Household.GetInvitations);
    }

    public async Task<Result> CancelInvitationAsync(Guid managerUserId, Guid invitationId, CancellationToken cancellationToken = default)
    {
        var managerMember = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(managerUserId, cancellationToken);
        if (managerMember == null)
        {
            return Result.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        if (managerMember.Role != Roles.HouseholdManager)
        {
            return Result.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var invitation = await _unitOfWork.HouseholdInvitations.GetByIdAndHouseholdIdAsync(invitationId, managerMember.HouseholdId, cancellationToken);
        if (invitation == null)
        {
            return Result.Fail(ErrorMessages.Household.InvitationNotFound, ResultStatus.NotFound);
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result.Fail(ErrorMessages.Household.OnlyPendingCanBeCancelled, ResultStatus.BadRequest);
        }

        invitation.Status = InvitationStatus.Cancelled;
        _unitOfWork.HouseholdInvitations.Update(invitation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Household.CancelInvitation);
    }

    public async Task<Result> AcceptInvitationAsync(Guid userId, Guid invitationId, CancellationToken cancellationToken = default)
    {
        var isAlreadyInHousehold = await _unitOfWork.HouseholdMembers.IsUserInAnyHouseholdAsync(userId, cancellationToken);
        if (isAlreadyInHousehold)
        {
            return Result.Fail(ErrorMessages.Household.AlreadyInHousehold, ResultStatus.BadRequest);
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Fail(ErrorMessages.Household.UserNotFound, ResultStatus.NotFound);
        }

        var invitation = await _unitOfWork.HouseholdInvitations.GetByIdAsync(invitationId, cancellationToken);
        if (invitation == null)
        {
            return Result.Fail(ErrorMessages.Household.InvitationNotFound, ResultStatus.NotFound);
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result.Fail(ErrorMessages.Household.InvitationExpiredOrInvalid, ResultStatus.BadRequest);
        }

        bool isTargetUser = (invitation.InvitedEmail != null && invitation.InvitedEmail.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                         || (invitation.InvitedUserName != null && invitation.InvitedUserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase));

        if (!isTargetUser)
        {
            return Result.Fail(ErrorMessages.Household.InvitationNotForUser, ResultStatus.Forbidden);
        }

        invitation.Status = InvitationStatus.Accepted;
        _unitOfWork.HouseholdInvitations.Update(invitation);

        var unlinkedMember = await _unitOfWork.HouseholdMembers.FindUnlinkedMemberAsync(invitation.HouseholdId, user.FullName, cancellationToken);
        if (unlinkedMember != null)
        {
            unlinkedMember.UserId = userId;
            unlinkedMember.Role = Roles.HouseholdMember;
            _unitOfWork.HouseholdMembers.Update(unlinkedMember);
        }
        else
        {
            var newMember = new HouseholdMember
            {
                HouseholdId = invitation.HouseholdId,
                UserId = userId,
                FullName = string.IsNullOrWhiteSpace(user.FullName) ? user.UserName ?? "Member" : user.FullName,
                Gender = user.Gender,
                DateOfBirth = user.BirthDate,
                Role = Roles.HouseholdMember,
                JoinedAt = DateTime.UtcNow
            };
            await _unitOfWork.HouseholdMembers.AddAsync(newMember, cancellationToken);
        }

        if (await _userManager.IsInRoleAsync(user, Roles.HouseholdManager))
        {
            await _userManager.RemoveFromRoleAsync(user, Roles.HouseholdManager);
        }
        if (!await _userManager.IsInRoleAsync(user, Roles.HouseholdMember))
        {
            await _userManager.AddToRoleAsync(user, Roles.HouseholdMember);
        }

        await _userManager.UpdateSecurityStampAsync(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Household.AcceptInvitation);
    }

    public async Task<Result> DeclineInvitationAsync(Guid userId, Guid invitationId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Fail(ErrorMessages.Household.UserNotFound, ResultStatus.NotFound);
        }

        var invitation = await _unitOfWork.HouseholdInvitations.GetByIdAsync(invitationId, cancellationToken);
        if (invitation == null)
        {
            return Result.Fail(ErrorMessages.Household.InvitationNotFound, ResultStatus.NotFound);
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result.Fail(ErrorMessages.Household.InvitationExpiredOrInvalid, ResultStatus.BadRequest);
        }

        bool isTargetUser = (invitation.InvitedEmail != null && invitation.InvitedEmail.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                         || (invitation.InvitedUserName != null && invitation.InvitedUserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase));

        if (!isTargetUser)
        {
            return Result.Fail(ErrorMessages.Household.InvitationNotForUser, ResultStatus.Forbidden);
        }

        invitation.Status = InvitationStatus.Declined;
        _unitOfWork.HouseholdInvitations.Update(invitation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Household.DeclineInvitation);
    }
}
