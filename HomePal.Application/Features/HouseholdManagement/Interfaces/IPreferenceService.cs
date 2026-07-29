using HomePal.Application.Features.HouseholdManagement.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.HouseholdManagement.Interfaces;

public interface IPreferenceService
{
    Task<Result<IReadOnlyCollection<PreferenceResponse>>> GetAllPreferencesAsync(Guid? categoryId = null, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PreferenceResponse>>> SearchPreferencesAsync(string? query, Guid? categoryId = null, CancellationToken cancellationToken = default);
    Task<Result<PreferenceResponse>> GetPreferenceByIdAsync(Guid preferenceId, CancellationToken cancellationToken = default);
    Task<Result<PreferenceResponse>> CreatePreferenceAsync(Guid userId, AddPreferenceRequest request, CancellationToken cancellationToken = default);
    Task<Result<PreferenceResponse>> UpdatePreferenceAsync(Guid userId, Guid preferenceId, UpdatePreferenceRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeletePreferenceAsync(Guid userId, Guid preferenceId, CancellationToken cancellationToken = default);
}
