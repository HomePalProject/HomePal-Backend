using HomePal.Application.Features.ShoppingList.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.ShoppingList.Interfaces;

public interface IShoppingListService
{
    Task<Result<List<ShoppingListItemResponse>>> GetShoppingListAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ShoppingListItemResponse>> AddCustomItemAsync(Guid userId, CreateShoppingListItemRequest request, CancellationToken cancellationToken = default);
    Task<Result<ShoppingListItemResponse>> AddFromOfferAsync(Guid userId, AddFromOfferRequest request, CancellationToken cancellationToken = default);
    Task<Result<ShoppingListItemResponse>> UpdateItemAsync(Guid userId, Guid itemId, UpdateShoppingListItemRequest request, CancellationToken cancellationToken = default);
    Task<Result<ShoppingListItemResponse>> TogglePurchasedAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
    Task<Result> DeleteItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
    Task<Result> ClearPurchasedAsync(Guid userId, CancellationToken cancellationToken = default);
}
