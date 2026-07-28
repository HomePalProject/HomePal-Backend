using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Mappers;
using HomePal.Domain.Constants;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Services;

public class MemberPreferenceService : IMemberPreferenceService
{
    private readonly IHouseholdMemberRepository _memberRepository;
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MemberPreferenceService(
        IHouseholdMemberRepository memberRepository,
        IPreferenceRepository preferenceRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _preferenceRepository = preferenceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyCollection<PreferenceResponse>>> GetMemberPreferencesAsync(Guid currentUserId, Guid memberId, CancellationToken cancellationToken = default)
    {
        var currentMember = await _memberRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        if (currentMember == null)
        {
            return Result<IReadOnlyCollection<PreferenceResponse>>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        var targetMember = await _memberRepository.GetByIdWithPreferencesAsync(memberId, currentMember.HouseholdId, cancellationToken);
        if (targetMember == null)
        {
            return Result<IReadOnlyCollection<PreferenceResponse>>.Fail(ErrorMessages.Household.MemberNotFound, ResultStatus.NotFound);
        }

        return Result<IReadOnlyCollection<PreferenceResponse>>.Ok(targetMember.Preferences.ToResponseList(), SuccessMessages.Household.GetPreferences);
    }

    public async Task<Result<IReadOnlyCollection<PreferenceResponse>>> SetMemberPreferencesAsync(Guid currentUserId, Guid memberId, AssignPreferencesRequest request, CancellationToken cancellationToken = default)
    {
        var currentMember = await _memberRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        if (currentMember == null)
        {
            return Result<IReadOnlyCollection<PreferenceResponse>>.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        var targetMember = await _memberRepository.GetByIdWithPreferencesAsync(memberId, currentMember.HouseholdId, cancellationToken);
        if (targetMember == null)
        {
            return Result<IReadOnlyCollection<PreferenceResponse>>.Fail(ErrorMessages.Household.MemberNotFound, ResultStatus.NotFound);
        }

        bool isSelf = targetMember.UserId.HasValue && targetMember.UserId.Value == currentUserId;
        bool isManager = currentMember.Role == Roles.HouseholdManager;

        if (!isSelf && !isManager)
        {
            return Result<IReadOnlyCollection<PreferenceResponse>>.Fail(ErrorMessages.Household.PreferenceManagementUnauthorized, ResultStatus.Forbidden);
        }

        var selectedPreferences = await _preferenceRepository.GetByIdsAsync(request.PreferenceIds, cancellationToken);

        targetMember.Preferences.Clear();
        foreach (var pref in selectedPreferences)
        {
            targetMember.Preferences.Add(pref);
        }

        _memberRepository.Update(targetMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyCollection<PreferenceResponse>>.Ok(targetMember.Preferences.ToResponseList(), SuccessMessages.Household.SetPreferences);
    }

    public async Task<Result> RemoveMemberPreferenceAsync(Guid currentUserId, Guid memberId, Guid preferenceId, CancellationToken cancellationToken = default)
    {
        var currentMember = await _memberRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        if (currentMember == null)
        {
            return Result.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound);
        }

        var targetMember = await _memberRepository.GetByIdWithPreferencesAsync(memberId, currentMember.HouseholdId, cancellationToken);
        if (targetMember == null)
        {
            return Result.Fail(ErrorMessages.Household.MemberNotFound, ResultStatus.NotFound);
        }

        bool isSelf = targetMember.UserId.HasValue && targetMember.UserId.Value == currentUserId;
        bool isManager = currentMember.Role == Roles.HouseholdManager;

        if (!isSelf && !isManager)
        {
            return Result.Fail(ErrorMessages.Household.PreferenceManagementUnauthorized, ResultStatus.Forbidden);
        }

        var preference = targetMember.Preferences.FirstOrDefault(p => p.Id == preferenceId);
        if (preference == null)
        {
            return Result.Fail(ErrorMessages.Household.PreferenceNotFound, ResultStatus.NotFound);
        }

        targetMember.Preferences.Remove(preference);
        _memberRepository.Update(targetMember);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Household.RemovePreference);
    }
}
