using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Mappers;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Identity;

namespace HomePal.Application.Features.HouseholdManagement.Services;

public class HouseholdMemberService : IHouseholdMemberService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public HouseholdMemberService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyCollection<HouseholdMemberResponse>>> GetHouseholdMembersAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var currentMember = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        if (currentMember == null)
        {
            return Result<IReadOnlyCollection<HouseholdMemberResponse>>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        var members = await _unitOfWork.HouseholdMembers.GetByHouseholdIdAsync(currentMember.HouseholdId, cancellationToken);

        return Result<IReadOnlyCollection<HouseholdMemberResponse>>.Ok(members.ToResponseList(), SuccessMessages.Household.GetMembers);
    }

    public async Task<Result<HouseholdMemberResponse>> GetMemberByIdAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken = default)
    {
        var currentMember = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(currentUserId, cancellationToken);
        if (currentMember == null)
        {
            return Result<HouseholdMemberResponse>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        var targetMember = await _unitOfWork.HouseholdMembers.GetByIdAndHouseholdIdAsync(memberId, currentMember.HouseholdId, cancellationToken);
        if (targetMember == null)
        {
            return Result<HouseholdMemberResponse>.Fail(ErrorMessages.Household.MemberNotFound, ResultStatus.NotFound);
        }

        return Result<HouseholdMemberResponse>.Ok(targetMember.ToResponse(), SuccessMessages.Household.GetMember);
    }

    public async Task<Result<HouseholdMemberResponse>> AddOfflineMemberAsync(Guid managerUserId, AddOfflineMemberRequest request, CancellationToken cancellationToken = default)
    {
        var managerMember = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(managerUserId, cancellationToken);
        if (managerMember == null)
        {
            return Result<HouseholdMemberResponse>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        if (managerMember.Role != Roles.HouseholdManager)
        {
            return Result<HouseholdMemberResponse>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var offlineMember = request.ToOfflineMemberEntity(managerMember.HouseholdId);

        await _unitOfWork.HouseholdMembers.AddAsync(offlineMember, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<HouseholdMemberResponse>.Ok(offlineMember.ToResponse(), SuccessMessages.Household.AddMember, ResultStatus.Created);
    }

    public async Task<Result<HouseholdMemberResponse>> UpdateMemberAsync(Guid managerUserId, Guid memberId, UpdateMemberRequest request, CancellationToken cancellationToken = default)
    {
        var managerMember = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(managerUserId, cancellationToken);
        if (managerMember == null)
        {
            return Result<HouseholdMemberResponse>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        if (managerMember.Role != Roles.HouseholdManager)
        {
            return Result<HouseholdMemberResponse>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var targetMember = await _unitOfWork.HouseholdMembers.GetByIdAndHouseholdIdAsync(memberId, managerMember.HouseholdId, cancellationToken);
        if (targetMember == null)
        {
            return Result<HouseholdMemberResponse>.Fail(ErrorMessages.Household.MemberNotFound, ResultStatus.NotFound);
        }

        targetMember.FullName = request.FullName.Trim();
        targetMember.Gender = request.Gender;
        targetMember.DateOfBirth = request.DateOfBirth;

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var newRole = request.Role.Trim();
            targetMember.Role = newRole;

            if (targetMember.UserId.HasValue)
            {
                var targetUser = await _userManager.FindByIdAsync(targetMember.UserId.Value.ToString());
                if (targetUser != null)
                {
                    if (newRole == Roles.HouseholdManager)
                    {
                        await _userManager.RemoveFromRoleAsync(targetUser, Roles.HouseholdMember);
                        if (!await _userManager.IsInRoleAsync(targetUser, Roles.HouseholdManager))
                        {
                            await _userManager.AddToRoleAsync(targetUser, Roles.HouseholdManager);
                        }
                    }
                    else if (newRole == Roles.HouseholdMember)
                    {
                        await _userManager.RemoveFromRoleAsync(targetUser, Roles.HouseholdManager);
                        if (!await _userManager.IsInRoleAsync(targetUser, Roles.HouseholdMember))
                        {
                            await _userManager.AddToRoleAsync(targetUser, Roles.HouseholdMember);
                        }
                    }
                }
            }
        }

        _unitOfWork.HouseholdMembers.Update(targetMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<HouseholdMemberResponse>.Ok(targetMember.ToResponse(), SuccessMessages.Household.UpdateMember);
    }

    public async Task<Result> RemoveMemberAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken = default)
    {
        var currentMember = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(currentUserId, cancellationToken);
        if (currentMember == null)
        {
            return Result.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        var targetMember = await _unitOfWork.HouseholdMembers.GetByIdAndHouseholdIdAsync(memberId, currentMember.HouseholdId, cancellationToken);
        if (targetMember == null)
        {
            return Result.Fail(ErrorMessages.Household.MemberNotFound, ResultStatus.NotFound);
        }

        bool isSelf = targetMember.UserId == currentUserId;
        bool isManager = currentMember.Role == Roles.HouseholdManager;

        if (!isManager && !isSelf)
        {
            return Result.Fail(ErrorMessages.Household.MemberRemovalUnauthorized, ResultStatus.Forbidden);
        }

        // Household Manager Protection Rules:
        // 1. A Household Manager cannot remove themselves from the household (must use DELETE /api/households to disband).
        // 2. A Household Manager cannot remove another manager if they are the only manager remaining.
        if (targetMember.Role == Roles.HouseholdManager)
        {
            if (isSelf)
            {
                return Result.Fail(ErrorMessages.Household.ManagerCannotRemoveSelf, ResultStatus.BadRequest);
            }

            int managerCount = await _unitOfWork.HouseholdMembers.GetManagerCountAsync(currentMember.HouseholdId, cancellationToken);
            if (managerCount <= 1)
            {
                return Result.Fail(ErrorMessages.Household.CannotRemoveOnlyManager, ResultStatus.BadRequest);
            }
        }

        if (targetMember.UserId.HasValue)
        {
            var targetUser = await _userManager.FindByIdAsync(targetMember.UserId.Value.ToString());
            if (targetUser != null)
            {
                if (await _userManager.IsInRoleAsync(targetUser, Roles.HouseholdMember))
                {
                    await _userManager.RemoveFromRoleAsync(targetUser, Roles.HouseholdMember);
                }
                if (!await _userManager.IsInRoleAsync(targetUser, Roles.HouseholdManager))
                {
                    await _userManager.AddToRoleAsync(targetUser, Roles.HouseholdManager);
                }
            }
        }

        _unitOfWork.HouseholdMembers.Remove(targetMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Household.RemoveMember);
    }
}
