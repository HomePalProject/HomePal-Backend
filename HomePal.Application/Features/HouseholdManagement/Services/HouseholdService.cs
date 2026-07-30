using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Mappers;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Identity;

namespace HomePal.Application.Features.HouseholdManagement.Services;

public class HouseholdService : IHouseholdService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public HouseholdService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HouseholdResponse>> CreateHouseholdAsync(Guid userId, CreateHouseholdRequest request, CancellationToken cancellationToken = default)
    {
        var existingMembership = await _unitOfWork.HouseholdMembers.IsUserInAnyHouseholdAsync(userId, cancellationToken);
        if (existingMembership)
        {
            return Result<HouseholdResponse>.Fail(ErrorMessages.Household.AlreadyInHousehold, ResultStatus.BadRequest);
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<HouseholdResponse>.Fail(ErrorMessages.Household.UserNotFound, ResultStatus.NotFound);
        }

        var household = request.ToEntity();
        await _unitOfWork.Households.AddAsync(household, cancellationToken);

        var creatorMember = new HouseholdMember
        {
            HouseholdId = household.Id,
            UserId = userId,
            FullName = string.IsNullOrWhiteSpace(user.FullName) ? user.UserName ?? "Manager" : user.FullName,
            Gender = user.Gender,
            DateOfBirth = user.BirthDate,
            Role = Roles.HouseholdManager,
            JoinedAt = DateTime.UtcNow
        };

        await _unitOfWork.HouseholdMembers.AddAsync(creatorMember, cancellationToken);

        if (!await _userManager.IsInRoleAsync(user, Roles.HouseholdManager))
        {
            await _userManager.AddToRoleAsync(user, Roles.HouseholdManager);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<HouseholdResponse>.Ok(household.ToResponse(1), SuccessMessages.Household.Create, ResultStatus.Created);
    }

    public async Task<Result<HouseholdResponse>> GetMyHouseholdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var household = await _unitOfWork.Households.GetByMemberUserIdAsync(userId, cancellationToken);
        if (household == null)
        {
            return Result<HouseholdResponse>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        var response = household.ToResponse(household.Members?.Count ?? 0);
        return Result<HouseholdResponse>.Ok(response, SuccessMessages.Household.Get);
    }

    public async Task<Result<HouseholdResponse>> UpdateHouseholdAsync(Guid userId, UpdateHouseholdRequest request, CancellationToken cancellationToken = default)
    {
        var member = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        if (member == null || member.Household == null)
        {
            return Result<HouseholdResponse>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result<HouseholdResponse>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var household = member.Household;
        household.Name = request.Name.Trim();
        household.Address = request.Address?.Trim();
        household.Governorate = request.Governorate?.Trim();
        household.City = request.City?.Trim();
        household.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Households.Update(household);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = household.ToResponse(household.Members?.Count ?? 0);
        return Result<HouseholdResponse>.Ok(response, SuccessMessages.Household.Update);
    }

    public async Task<Result> DeleteHouseholdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var member = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        if (member == null || member.Household == null)
        {
            return Result.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var household = member.Household;

        var members = await _unitOfWork.HouseholdMembers.GetByHouseholdIdAsync(household.Id, cancellationToken);
        foreach (var memberItem in members)
        {
            if (memberItem.UserId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(memberItem.UserId.Value.ToString());
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, Roles.HouseholdMember))
                    {
                        await _userManager.RemoveFromRoleAsync(user, Roles.HouseholdMember);
                    }
                    if (!await _userManager.IsInRoleAsync(user, Roles.HouseholdManager))
                    {
                        await _userManager.AddToRoleAsync(user, Roles.HouseholdManager);
                    }
                    await _userManager.UpdateSecurityStampAsync(user);
                }
            }
        }

        _unitOfWork.Households.Remove(household);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Household.Delete);
    }
}
