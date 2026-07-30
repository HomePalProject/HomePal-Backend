using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.PantryManagement.Interfaces;

public interface IPantryItemService
{
    Task<Result<IReadOnlyCollection<PantryItemResponse>>> GetPantryItemsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PantryItemResponse>> GetPantryItemByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<PantryItemResponse>> CreatePantryItemAsync(Guid userId, CreatePantryItemRequest request, CancellationToken cancellationToken = default);
    Task<Result<PantryItemResponse>> UpdatePantryItemAsync(Guid userId, Guid itemId, UpdatePantryItemRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeletePantryItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
}
