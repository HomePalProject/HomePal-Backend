using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Application.Features.PantryManagement.Mappers;
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

    private async Task<(Pantry? Pantry, Result? ErrorResult)> GetUserPantryAsync(Guid userId, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        if (member == null)
        {
            return (null, Result.Fail(ErrorMessages.Household.HouseholdNotFound, ResultStatus.NotFound));
        }

        var pantry = await _unitOfWork.Pantries.GetByHouseholdIdAsync(member.HouseholdId, cancellationToken);
        if (pantry == null)
        {
            pantry = new Pantry { HouseholdId = member.HouseholdId, CreatedAt = DateTime.UtcNow };
            await _unitOfWork.Pantries.AddAsync(pantry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return (pantry, null);
    }

    public async Task<Result<IReadOnlyCollection<PantryItemResponse>>> GetPantryItemsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var (pantry, error) = await GetUserPantryAsync(userId, cancellationToken);
        if (error != null)
        {
            return Result<IReadOnlyCollection<PantryItemResponse>>.Fail(error.Message, error.Status);
        }

        var items = await _unitOfWork.PantryItems.GetByPantryIdAsync(pantry!.Id, cancellationToken);
        var responses = items.Select(i => i.ToResponse()).ToList().AsReadOnly();

        return Result<IReadOnlyCollection<PantryItemResponse>>.Ok(responses, SuccessMessages.Pantry.GetItems);
    }

    public async Task<Result<PantryItemResponse>> GetPantryItemByIdAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var (pantry, error) = await GetUserPantryAsync(userId, cancellationToken);
        if (error != null)
        {
            return Result<PantryItemResponse>.Fail(error.Message, error.Status);
        }

        var item = await _unitOfWork.PantryItems.GetByIdAndPantryIdAsync(itemId, pantry!.Id, cancellationToken);
        if (item == null)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Pantry.ItemNotFound, ResultStatus.NotFound);
        }

        return Result<PantryItemResponse>.Ok(item.ToResponse(), SuccessMessages.Pantry.GetItem);
    }

    public async Task<Result<PantryItemResponse>> CreatePantryItemAsync(Guid userId, CreatePantryItemRequest request, CancellationToken cancellationToken = default)
    {
        var (pantry, error) = await GetUserPantryAsync(userId, cancellationToken);
        if (error != null)
        {
            return Result<PantryItemResponse>.Fail(error.Message, error.Status);
        }

        var entity = request.ToEntity(pantry!.Id);
        await _unitOfWork.PantryItems.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PantryItemResponse>.Ok(entity.ToResponse(), SuccessMessages.Pantry.CreateItem, ResultStatus.Created);
    }

    public async Task<Result<PantryItemResponse>> UpdatePantryItemAsync(Guid userId, Guid itemId, UpdatePantryItemRequest request, CancellationToken cancellationToken = default)
    {
        var (pantry, error) = await GetUserPantryAsync(userId, cancellationToken);
        if (error != null)
        {
            return Result<PantryItemResponse>.Fail(error.Message, error.Status);
        }

        var item = await _unitOfWork.PantryItems.GetByIdAndPantryIdAsync(itemId, pantry!.Id, cancellationToken);
        if (item == null)
        {
            return Result<PantryItemResponse>.Fail(ErrorMessages.Pantry.ItemNotFound, ResultStatus.NotFound);
        }

        item.Name = request.Name.Trim();
        item.ExpireDate = request.ExpireDate;
        item.Quantity = request.Quantity;
        item.MeasuringUnit = request.MeasuringUnit.Trim();
        item.Category = request.Category.Trim();
        item.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.PantryItems.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PantryItemResponse>.Ok(item.ToResponse(), SuccessMessages.Pantry.UpdateItem);
    }

    public async Task<Result> DeletePantryItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var (pantry, error) = await GetUserPantryAsync(userId, cancellationToken);
        if (error != null)
        {
            return Result.Fail(error.Message, error.Status);
        }

        var item = await _unitOfWork.PantryItems.GetByIdAndPantryIdAsync(itemId, pantry!.Id, cancellationToken);
        if (item == null)
        {
            return Result.Fail(ErrorMessages.Pantry.ItemNotFound, ResultStatus.NotFound);
        }

        _unitOfWork.PantryItems.Remove(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Pantry.DeleteItem);
    }
}
