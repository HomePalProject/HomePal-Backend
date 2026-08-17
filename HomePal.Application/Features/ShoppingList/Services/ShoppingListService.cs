using System.Globalization;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.ShoppingList.DTOs;
using HomePal.Application.Features.ShoppingList.Interfaces;
using HomePal.Application.Features.ShoppingList.Mappers;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.ShoppingList.Services;

public class ShoppingListService : IShoppingListService
{
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingListService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ShoppingListItemResponse>>> GetShoppingListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<List<ShoppingListItemResponse>>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var shoppingList = await _unitOfWork.ShoppingLists.GetOrCreateByHouseholdIdAsync(householdId.Value, cancellationToken);
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var items = await _unitOfWork.ShoppingListItems.GetByShoppingListIdAsync(shoppingList.Id, cancellationToken);
        var dtos = items.Select(i => i.ToResponse(culture)).ToList();

        return Result<List<ShoppingListItemResponse>>.Ok(dtos, SuccessMessages.General);
    }

    public async Task<Result<ShoppingListItemResponse>> AddCustomItemAsync(Guid userId, CreateShoppingListItemRequest request, CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var shoppingList = await _unitOfWork.ShoppingLists.GetOrCreateByHouseholdIdAsync(householdId.Value, cancellationToken);

        if (request.UnitId.HasValue)
        {
            var unit = await _unitOfWork.MeasuringUnits.GetByIdAsync(request.UnitId.Value, cancellationToken);
            if (unit == null)
                return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.BadRequest);
        }

        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category == null)
                return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
        }

        var newItem = request.ToEntity(shoppingList.Id);

        await _unitOfWork.ShoppingListItems.AddAsync(newItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var savedItem = await _unitOfWork.ShoppingListItems.GetByIdWithDetailsAsync(newItem.Id, cancellationToken) ?? newItem;
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return Result<ShoppingListItemResponse>.Ok(savedItem.ToResponse(culture), SuccessMessages.General, ResultStatus.Created);
    }

    public async Task<Result<ShoppingListItemResponse>> AddFromOfferAsync(Guid userId, AddFromOfferRequest request, CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var shoppingList = await _unitOfWork.ShoppingLists.GetOrCreateByHouseholdIdAsync(householdId.Value, cancellationToken);

        var offer = await _unitOfWork.Offers.GetByIdWithDetailsAsync(request.OfferId, cancellationToken);
        if (offer == null)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Catalog.OfferNotFound, ResultStatus.NotFound);
        }

        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var newItem = offer.ToEntity(shoppingList.Id, request.CustomQuantity, request.PortionCount, request.Notes, culture);

        await _unitOfWork.ShoppingListItems.AddAsync(newItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var savedItem = await _unitOfWork.ShoppingListItems.GetByIdWithDetailsAsync(newItem.Id, cancellationToken) ?? newItem;
        return Result<ShoppingListItemResponse>.Ok(savedItem.ToResponse(culture), SuccessMessages.General, ResultStatus.Created);
    }

    public async Task<Result<ShoppingListItemResponse>> UpdateItemAsync(Guid userId, Guid itemId, UpdateShoppingListItemRequest request, CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var shoppingList = await _unitOfWork.ShoppingLists.GetByHouseholdIdAsync(householdId.Value, cancellationToken);
        if (shoppingList == null)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.General, ResultStatus.NotFound);
        }

        var item = await _unitOfWork.ShoppingListItems.GetByIdWithDetailsAsync(itemId, cancellationToken);
        if (item == null || item.ShoppingListId != shoppingList.Id)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.General, ResultStatus.NotFound);
        }

        if (request.UnitId.HasValue)
        {
            var unit = await _unitOfWork.MeasuringUnits.GetByIdAsync(request.UnitId.Value, cancellationToken);
            if (unit == null)
                return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.BadRequest);
        }

        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category == null)
                return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
        }

        item.UpdateEntity(request);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedItem = await _unitOfWork.ShoppingListItems.GetByIdWithDetailsAsync(item.Id, cancellationToken) ?? item;
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return Result<ShoppingListItemResponse>.Ok(updatedItem.ToResponse(culture), SuccessMessages.General);
    }

    public async Task<Result<ShoppingListItemResponse>> TogglePurchasedAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var shoppingList = await _unitOfWork.ShoppingLists.GetByHouseholdIdAsync(householdId.Value, cancellationToken);
        if (shoppingList == null)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.General, ResultStatus.NotFound);
        }

        var item = await _unitOfWork.ShoppingListItems.GetByIdWithDetailsAsync(itemId, cancellationToken);
        if (item == null || item.ShoppingListId != shoppingList.Id)
        {
            return Result<ShoppingListItemResponse>.Fail(ErrorMessages.General, ResultStatus.NotFound);
        }

        item.IsPurchased = !item.IsPurchased;
        item.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return Result<ShoppingListItemResponse>.Ok(item.ToResponse(culture), SuccessMessages.General);
    }

    public async Task<Result> DeleteItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var shoppingList = await _unitOfWork.ShoppingLists.GetByHouseholdIdAsync(householdId.Value, cancellationToken);
        if (shoppingList == null)
        {
            return Result.Fail(ErrorMessages.General, ResultStatus.NotFound);
        }

        var item = await _unitOfWork.ShoppingListItems.GetByIdAsync(itemId, cancellationToken);
        if (item == null || item.ShoppingListId != shoppingList.Id)
        {
            return Result.Fail(ErrorMessages.General, ResultStatus.NotFound);
        }

        _unitOfWork.ShoppingListItems.Remove(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.General);
    }

    public async Task<Result> ClearPurchasedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var householdId = await GetUserHouseholdIdAsync(userId, cancellationToken);
        if (!householdId.HasValue)
        {
            return Result.Fail(ErrorMessages.Pantry.NoHousehold, ResultStatus.BadRequest);
        }

        var shoppingList = await _unitOfWork.ShoppingLists.GetByHouseholdIdAsync(householdId.Value, cancellationToken);
        if (shoppingList == null)
        {
            return Result.Ok(SuccessMessages.General);
        }

        var purchasedItems = shoppingList.Items.Where(i => i.IsPurchased).ToList();
        if (purchasedItems.Count > 0)
        {
            var now = DateTime.UtcNow;
            var currentBudget = await _unitOfWork.MonthlyBudgets.GetByHouseholdAndPeriodAsync(householdId.Value, now.Year, now.Month, cancellationToken);

            var newExpenses = purchasedItems
                .Where(i => i.Price.HasValue && i.Price.Value > 0)
                .Select(i => new HouseholdExpense
                {
                    Id = Guid.NewGuid(),
                    HouseholdId = householdId.Value,
                    BudgetId = currentBudget?.Id,
                    Title = i.Name,
                    Amount = i.Price!.Value * (decimal)(i.PortionCount > 0 ? i.PortionCount : 1),
                    ExpenseDate = now,
                    CreatedAt = now
                })
                .ToList();

            if (newExpenses.Count > 0)
            {
                await _unitOfWork.HouseholdExpenses.AddRangeAsync(newExpenses, cancellationToken);
            }

            var pantry = await _unitOfWork.Pantries.GetByHouseholdIdAsync(householdId.Value, cancellationToken);
            if (pantry == null)
            {
                return Result.Fail(ErrorMessages.Pantry.PantryNotFound, ResultStatus.NotFound);
            }

            var newPantryItems = purchasedItems.Select(i => new PantryItem
            {
                Id = Guid.NewGuid(),
                PantryId = pantry.Id,
                Name = i.Name,
                Quantity = (decimal)((i.Quantity > 0 ? i.Quantity : 1) * (i.PortionCount > 0 ? i.PortionCount : 1)),
                MeasuringUnitId = i.MeasuringUnitId,
                CategoryId = i.CategoryId,
                CreatedAt = now
            }).ToList();

            await _unitOfWork.PantryItems.AddRangeAsync(newPantryItems, cancellationToken);
        }

        await _unitOfWork.ShoppingListItems.ClearPurchasedAsync(shoppingList.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.General);
    }

    private async Task<Guid?> GetUserHouseholdIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        return member?.HouseholdId;
    }
}
