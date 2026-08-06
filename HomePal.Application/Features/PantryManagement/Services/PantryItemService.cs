using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Application.Features.PantryManagement.Mappers;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.PantryManagement.Services;

public class PantryItemService : IPantryItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public PantryItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private async Task<(HouseholdMember? Member, Pantry? Pantry, string? ErrorMessage, ResultStatus Status)> GetOrCreatePantryForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        if (member == null)
        {
            return (null, null, ErrorMessages.Pantry.NoHousehold, ResultStatus.NotFound);
        }

        var pantry = await _unitOfWork.Pantries.GetByHouseholdIdAsync(member.HouseholdId, cancellationToken);
        if (pantry == null)
        {
            pantry = new Pantry
            {
                Id = Guid.NewGuid(),
                HouseholdId = member.HouseholdId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Pantries.AddAsync(pantry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return (member, pantry, null, ResultStatus.Success);
    }

    public async Task<Result<IReadOnlyList<PantryItemResponse>>> GetPantryItemsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var (member, pantry, errorMsg, status) = await GetOrCreatePantryForUserAsync(userId, cancellationToken);
        if (errorMsg != null || pantry == null)
        {
            return Result<IReadOnlyList<PantryItemResponse>>.Fail(errorMsg ?? ErrorMessages.Pantry.NoHousehold, status);
        }

        var items = await _unitOfWork.PantryItems.GetByPantryIdAsync(pantry.Id, cancellationToken);
        var responseList = items.Select(i => i.ToResponse()).ToList();

        return Result<IReadOnlyList<PantryItemResponse>>.Ok(responseList, SuccessMessages.Pantry.GetItems);
    }

    public async Task<Result<PantryItemResponse>> GetPantryItemByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var (member, pantry, errorMsg, status) = await GetOrCreatePantryForUserAsync(userId, cancellationToken);
        if (errorMsg != null || pantry == null)
        {
            return Result<PantryItemResponse>.Fail(errorMsg ?? ErrorMessages.Pantry.NoHousehold, status);
        }

        var item = await _unitOfWork.PantryItems.GetByIdAndPantryIdAsync(itemId, pantry.Id, cancellationToken);
        if (item == null)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Pantry.PantryItemNotFound, ResultStatus.NotFound);
        }

        return Result<PantryItemResponse>.Ok(item.ToResponse(), SuccessMessages.Pantry.GetItem);
    }

    public async Task<Result<PantryItemResponse>> CreatePantryItemAsync(Guid userId, CreatePantryItemRequest request, CancellationToken cancellationToken = default)
    {
        var (member, pantry, errorMsg, status) = await GetOrCreatePantryForUserAsync(userId, cancellationToken);
        if (errorMsg != null || member == null || pantry == null)
        {
            return Result<PantryItemResponse>.Fail(errorMsg ?? ErrorMessages.Pantry.NoHousehold, status);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var measuringUnit = await _unitOfWork.MeasuringUnits.GetByIdAsync(request.MeasuringUnitId, cancellationToken);
        if (measuringUnit == null)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.BadRequest);
        }

        var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
        }

        var item = request.ToEntity(pantry.Id);
        await _unitOfWork.PantryItems.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdItem = await _unitOfWork.PantryItems.GetByIdAndPantryIdAsync(item.Id, pantry.Id, cancellationToken);
        return Result<PantryItemResponse>.Ok(createdItem!.ToResponse(), SuccessMessages.Pantry.CreateItem, ResultStatus.Created);
    }

    public async Task<Result<PantryItemResponse>> UpdatePantryItemAsync(Guid userId, Guid itemId, UpdatePantryItemRequest request, CancellationToken cancellationToken = default)
    {
        var (member, pantry, errorMsg, status) = await GetOrCreatePantryForUserAsync(userId, cancellationToken);
        if (errorMsg != null || member == null || pantry == null)
        {
            return Result<PantryItemResponse>.Fail(errorMsg ?? ErrorMessages.Pantry.NoHousehold, status);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var item = await _unitOfWork.PantryItems.GetByIdAndPantryIdAsync(itemId, pantry.Id, cancellationToken);
        if (item == null)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Pantry.PantryItemNotFound, ResultStatus.NotFound);
        }

        var measuringUnit = await _unitOfWork.MeasuringUnits.GetByIdAsync(request.MeasuringUnitId, cancellationToken);
        if (measuringUnit == null)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.BadRequest);
        }

        var category = await _unitOfWork.ProductCategories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category == null)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
        }

        item.Name = request.Name.Trim();
        item.ExpireDate = request.ExpireDate;
        item.Quantity = request.Quantity;
        item.MeasuringUnitId = request.MeasuringUnitId;
        item.CategoryId = request.CategoryId;
        item.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.PantryItems.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedItem = await _unitOfWork.PantryItems.GetByIdAndPantryIdAsync(item.Id, pantry.Id, cancellationToken);
        return Result<PantryItemResponse>.Ok(updatedItem!.ToResponse(), SuccessMessages.Pantry.UpdateItem);
    }

    public async Task<Result<IReadOnlyList<PantryItemResponse>>> UpdateEntirePantryItemsAsync(Guid userId, UpdateEntirePantryItemsRequest request, CancellationToken cancellationToken = default)
    {
        var (member, pantry, errorMsg, status) = await GetOrCreatePantryForUserAsync(userId, cancellationToken);
        if (errorMsg != null || member == null || pantry == null)
        {
            return Result<IReadOnlyList<PantryItemResponse>>.Fail(errorMsg ?? ErrorMessages.Pantry.NoHousehold, status);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result<IReadOnlyList<PantryItemResponse>>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var existingItems = await _unitOfWork.PantryItems.GetByPantryIdAsync(pantry.Id, cancellationToken);
        if (existingItems.Count > 0)
        {
            _unitOfWork.PantryItems.RemoveRange(existingItems);
        }

        var newItems = new List<PantryItem>();
        foreach (var reqItem in request.Items)
        {
            var measuringUnit = await _unitOfWork.MeasuringUnits.GetByIdAsync(reqItem.MeasuringUnitId, cancellationToken);
            if (measuringUnit == null)
            {
                return Result<IReadOnlyList<PantryItemResponse>>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.BadRequest);
            }

            var category = await _unitOfWork.ProductCategories.GetByIdAsync(reqItem.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<IReadOnlyList<PantryItemResponse>>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
            }

            newItems.Add(reqItem.ToEntity(pantry.Id));
        }

        if (newItems.Count > 0)
        {
            await _unitOfWork.PantryItems.AddRangeAsync(newItems, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedList = await _unitOfWork.PantryItems.GetByPantryIdAsync(pantry.Id, cancellationToken);
        var responseList = updatedList.Select(i => i.ToResponse()).ToList();

        return Result<IReadOnlyList<PantryItemResponse>>.Ok(responseList, SuccessMessages.Pantry.UpdateEntireItems);
    }

    public async Task<Result> DeletePantryItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var (member, pantry, errorMsg, status) = await GetOrCreatePantryForUserAsync(userId, cancellationToken);
        if (errorMsg != null || member == null || pantry == null)
        {
            return Result.Fail(errorMsg ?? ErrorMessages.Pantry.NoHousehold, status);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var item = await _unitOfWork.PantryItems.GetByIdAndPantryIdAsync(itemId, pantry.Id, cancellationToken);
        if (item == null)
        {
            return Result.Fail(ErrorMessages.Pantry.PantryItemNotFound, ResultStatus.NotFound);
        }

        _unitOfWork.PantryItems.Remove(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Pantry.DeleteItem);
    }

    public async Task<Result<PantryScanResponse>> ScanPantryCameraAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var (member, pantry, errorMsg, status) = await GetOrCreatePantryForUserAsync(userId, cancellationToken);
        if (errorMsg != null || pantry == null)
        {
            return Result<PantryScanResponse>.Fail(errorMsg ?? ErrorMessages.Pantry.NoHousehold, status);
        }

        var measuringUnits = await _unitOfWork.MeasuringUnits.GetAllAsync(cancellationToken);
        var categories = await _unitOfWork.ProductCategories.GetAllAsync(cancellationToken);

        var defaultUnit = measuringUnits.FirstOrDefault();
        var defaultCategory = categories.FirstOrDefault();

        var dummyScanItems = new List<PantryScanItemDto>
        {
            new PantryScanItemDto
            {
                Name = "Fresh Milk 1L",
                Quantity = 2,
                MeasuringUnitId = defaultUnit?.Id ?? Guid.NewGuid(),
                MeasuringUnitName = defaultUnit?.Name,
                CategoryId = defaultCategory?.Id ?? Guid.NewGuid(),
                CategoryName = defaultCategory?.Name,
                SuggestedExpireDate = DateTime.UtcNow.AddDays(7)
            },
            new PantryScanItemDto
            {
                Name = "Greek Yogurt 500g",
                Quantity = 1,
                MeasuringUnitId = defaultUnit?.Id ?? Guid.NewGuid(),
                MeasuringUnitName = defaultUnit?.Name,
                CategoryId = defaultCategory?.Id ?? Guid.NewGuid(),
                CategoryName = defaultCategory?.Name,
                SuggestedExpireDate = DateTime.UtcNow.AddDays(14)
            },
            new PantryScanItemDto
            {
                Name = "Cheddar Cheese 200g",
                Quantity = 3,
                MeasuringUnitId = defaultUnit?.Id ?? Guid.NewGuid(),
                MeasuringUnitName = defaultUnit?.Name,
                CategoryId = defaultCategory?.Id ?? Guid.NewGuid(),
                CategoryName = defaultCategory?.Name,
                SuggestedExpireDate = DateTime.UtcNow.AddDays(30)
            }
        };

        var response = new PantryScanResponse { Items = dummyScanItems };
        return Result<PantryScanResponse>.Ok(response, SuccessMessages.Pantry.Scan);
    }

    public async Task<Result<IReadOnlyList<PantryItemResponse>>> BulkAddPantryItemsAsync(Guid userId, BulkAddPantryItemsRequest request, CancellationToken cancellationToken = default)
    {
        var (member, pantry, errorMsg, status) = await GetOrCreatePantryForUserAsync(userId, cancellationToken);
        if (errorMsg != null || member == null || pantry == null)
        {
            return Result<IReadOnlyList<PantryItemResponse>>.Fail(errorMsg ?? ErrorMessages.Pantry.NoHousehold, status);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result<IReadOnlyList<PantryItemResponse>>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var newItems = new List<PantryItem>();
        foreach (var reqItem in request.Items)
        {
            var measuringUnit = await _unitOfWork.MeasuringUnits.GetByIdAsync(reqItem.MeasuringUnitId, cancellationToken);
            if (measuringUnit == null)
            {
                return Result<IReadOnlyList<PantryItemResponse>>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.BadRequest);
            }

            var category = await _unitOfWork.ProductCategories.GetByIdAsync(reqItem.CategoryId, cancellationToken);
            if (category == null)
            {
                return Result<IReadOnlyList<PantryItemResponse>>.Fail(ErrorMessages.Catalog.ProductCategoryNotFound, ResultStatus.BadRequest);
            }

            newItems.Add(reqItem.ToEntity(pantry.Id));
        }

        if (newItems.Count > 0)
        {
            await _unitOfWork.PantryItems.AddRangeAsync(newItems, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var currentItems = await _unitOfWork.PantryItems.GetByPantryIdAsync(pantry.Id, cancellationToken);
        var responseList = currentItems.Select(i => i.ToResponse()).ToList();

        return Result<IReadOnlyList<PantryItemResponse>>.Ok(responseList, SuccessMessages.Pantry.BulkAdd, ResultStatus.Created);
    }
}
