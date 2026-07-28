using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Application.Features.HouseholdManagement.Interfaces;
using HomePal.Application.Features.HouseholdManagement.Mappers;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Services;

public class PreferenceService : IPreferenceService
{
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PreferenceService(
        IPreferenceRepository preferenceRepository,
        IUnitOfWork unitOfWork)
    {
        _preferenceRepository = preferenceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyCollection<PreferenceResponse>>> GetAllPreferencesAsync(CancellationToken cancellationToken = default)
    {
        var preferences = await _preferenceRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<PreferenceResponse>>.Ok(preferences.ToResponseList(), SuccessMessages.Household.GetAllPreferences);
    }

    public async Task<Result<IReadOnlyCollection<PreferenceResponse>>> SearchPreferencesAsync(string? query, CancellationToken cancellationToken = default)
    {
        var preferences = await _preferenceRepository.SearchAsync(query ?? string.Empty, cancellationToken);
        return Result<IReadOnlyCollection<PreferenceResponse>>.Ok(preferences.ToResponseList(), SuccessMessages.Household.SearchPreferences);
    }

    public async Task<Result<PreferenceResponse>> GetPreferenceByIdAsync(Guid preferenceId, CancellationToken cancellationToken = default)
    {
        var preference = await _preferenceRepository.GetByIdAsync(preferenceId, cancellationToken);
        if (preference == null)
        {
            return Result<PreferenceResponse>.Fail(ErrorMessages.Household.PreferenceNotFound, ResultStatus.NotFound);
        }

        return Result<PreferenceResponse>.Ok(preference.ToResponse(), SuccessMessages.Household.GetPreference);
    }

    public async Task<Result<PreferenceResponse>> CreatePreferenceAsync(Guid userId, AddPreferenceRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _preferenceRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing != null)
        {
            return Result<PreferenceResponse>.Fail(ErrorMessages.Household.PreferenceAlreadyExists, ResultStatus.BadRequest);
        }

        var preference = new Preference
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _preferenceRepository.AddAsync(preference, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PreferenceResponse>.Ok(preference.ToResponse(), SuccessMessages.Household.AddPreference, ResultStatus.Created);
    }

    public async Task<Result<PreferenceResponse>> UpdatePreferenceAsync(Guid userId, Guid preferenceId, UpdatePreferenceRequest request, CancellationToken cancellationToken = default)
    {
        var preference = await _preferenceRepository.GetByIdAsync(preferenceId, cancellationToken);
        if (preference == null)
        {
            return Result<PreferenceResponse>.Fail(ErrorMessages.Household.PreferenceNotFound, ResultStatus.NotFound);
        }

        var nameTrimmed = request.Name.Trim();
        if (!preference.Name.Equals(nameTrimmed, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _preferenceRepository.GetByNameAsync(nameTrimmed, cancellationToken);
            if (existing != null && existing.Id != preferenceId)
            {
                return Result<PreferenceResponse>.Fail(ErrorMessages.Household.PreferenceAlreadyExists, ResultStatus.BadRequest);
            }
        }

        preference.Name = nameTrimmed;
        preference.Description = request.Description?.Trim();
        preference.UpdatedAt = DateTime.UtcNow;

        _preferenceRepository.Update(preference);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PreferenceResponse>.Ok(preference.ToResponse(), SuccessMessages.Household.UpdatePreference);
    }

    public async Task<Result> DeletePreferenceAsync(Guid userId, Guid preferenceId, CancellationToken cancellationToken = default)
    {
        var preference = await _preferenceRepository.GetByIdAsync(preferenceId, cancellationToken);
        if (preference == null)
        {
            return Result.Fail(ErrorMessages.Household.PreferenceNotFound, ResultStatus.NotFound);
        }

        _preferenceRepository.Remove(preference);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Household.DeletePreference);
    }
}
