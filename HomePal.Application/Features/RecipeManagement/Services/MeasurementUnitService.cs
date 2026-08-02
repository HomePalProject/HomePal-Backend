using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Application.Features.RecipeManagement.Mappers;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.RecipeManagement.Services;

public class MeasurementUnitService : IMeasurementUnitService
{
    private readonly IUnitOfWork _unitOfWork;

    public MeasurementUnitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<MeasurementUnitResponse>>> GetMeasurementUnitsAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default)
    {
        var paginatedUnits = await _unitOfWork.MeasurementUnits.GetPaginatedAsync(request, search, cancellationToken);
        var responseItems = paginatedUnits.Items.Select(m => m.ToResponse()).ToList();
        var result = PaginatedList<MeasurementUnitResponse>.Create(responseItems, paginatedUnits.TotalCount, paginatedUnits.PageNumber, request.PageSize);

        return Result<PaginatedList<MeasurementUnitResponse>>.Ok(result, SuccessMessages.MeasurementUnit.GetAll);
    }

    public async Task<Result<MeasurementUnitResponse>> GetMeasurementUnitByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unit = await _unitOfWork.MeasurementUnits.GetByIdAsync(id, cancellationToken);
        if (unit == null)
        {
            return Result<MeasurementUnitResponse>.Fail(ErrorMessages.MeasurementUnit.NotFound, ResultStatus.NotFound);
        }

        return Result<MeasurementUnitResponse>.Ok(unit.ToResponse(), SuccessMessages.MeasurementUnit.Get);
    }

    public async Task<Result<MeasurementUnitResponse>> CreateMeasurementUnitAsync(CreateMeasurementUnitRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Name == null || request.Name.Count == 0 || request.Name.All(n => string.IsNullOrWhiteSpace(n.Value)))
        {
            return Result<MeasurementUnitResponse>.Fail(ErrorMessages.MeasurementUnit.NameRequired, ResultStatus.BadRequest);
        }

        var unit = new MeasurementUnit
        {
            Name = request.Name,
            Symbol = request.Symbol,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.MeasurementUnits.AddAsync(unit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MeasurementUnitResponse>.Ok(unit.ToResponse(), SuccessMessages.MeasurementUnit.Create, ResultStatus.Created);
    }

    public async Task<Result<MeasurementUnitResponse>> UpdateMeasurementUnitAsync(Guid id, UpdateMeasurementUnitRequest request, CancellationToken cancellationToken = default)
    {
        var unit = await _unitOfWork.MeasurementUnits.GetByIdAsync(id, cancellationToken);
        if (unit == null)
        {
            return Result<MeasurementUnitResponse>.Fail(ErrorMessages.MeasurementUnit.NotFound, ResultStatus.NotFound);
        }

        if (request.Name == null || request.Name.Count == 0 || request.Name.All(n => string.IsNullOrWhiteSpace(n.Value)))
        {
            return Result<MeasurementUnitResponse>.Fail(ErrorMessages.MeasurementUnit.NameRequired, ResultStatus.BadRequest);
        }

        unit.Name = request.Name;
        unit.Symbol = request.Symbol;
        unit.Description = request.Description;
        unit.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.MeasurementUnits.Update(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MeasurementUnitResponse>.Ok(unit.ToResponse(), SuccessMessages.MeasurementUnit.Update);
    }

    public async Task<Result> DeleteMeasurementUnitAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unit = await _unitOfWork.MeasurementUnits.GetByIdAsync(id, cancellationToken);
        if (unit == null)
        {
            return Result.Fail(ErrorMessages.MeasurementUnit.NotFound, ResultStatus.NotFound);
        }

        var isUsed = await _unitOfWork.MeasurementUnits.IsUsedInRecipesAsync(id, cancellationToken);
        if (isUsed)
        {
            return Result.Fail(ErrorMessages.MeasurementUnit.InUse, ResultStatus.BadRequest);
        }

        _unitOfWork.MeasurementUnits.Remove(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.MeasurementUnit.Delete);
    }
}
