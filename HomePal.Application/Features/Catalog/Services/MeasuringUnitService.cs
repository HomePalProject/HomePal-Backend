using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;
using HomePal.Application.Features.Catalog.Mappers;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Catalog.Services;

public class MeasuringUnitService : IMeasuringUnitService
{
    private readonly IUnitOfWork _unitOfWork;

    public MeasuringUnitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyCollection<MeasuringUnitResponse>>> GetAllAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        var units = await _unitOfWork.MeasuringUnits.SearchAsync(query, cancellationToken);
        var responses = units.Select(u => u.ToResponse()).ToList();
        return Result<IReadOnlyCollection<MeasuringUnitResponse>>.Ok(responses, SuccessMessages.Catalog.GetAllMeasuringUnits);
    }

    public async Task<Result<MeasuringUnitResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unit = await _unitOfWork.MeasuringUnits.GetByIdAsync(id, cancellationToken);
        if (unit == null)
        {
            return Result<MeasuringUnitResponse>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.NotFound);
        }

        return Result<MeasuringUnitResponse>.Ok(unit.ToResponse(), SuccessMessages.Catalog.GetMeasuringUnit);
    }

    public async Task<Result<MeasuringUnitResponse>> CreateAsync(CreateMeasuringUnitRequest request, CancellationToken cancellationToken = default)
    {
        var unit = request.ToEntity();

        await _unitOfWork.MeasuringUnits.AddAsync(unit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MeasuringUnitResponse>.Ok(unit.ToResponse(), SuccessMessages.Catalog.CreateMeasuringUnit, ResultStatus.Created);
    }

    public async Task<Result<MeasuringUnitResponse>> UpdateAsync(Guid id, UpdateMeasuringUnitRequest request, CancellationToken cancellationToken = default)
    {
        var unit = await _unitOfWork.MeasuringUnits.GetByIdAsync(id, cancellationToken);
        if (unit == null)
        {
            return Result<MeasuringUnitResponse>.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.NotFound);
        }

        unit.UpdateEntity(request);

        _unitOfWork.MeasuringUnits.Update(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MeasuringUnitResponse>.Ok(unit.ToResponse(), SuccessMessages.Catalog.UpdateMeasuringUnit);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unit = await _unitOfWork.MeasuringUnits.GetByIdAsync(id, cancellationToken);
        if (unit == null)
        {
            return Result.Fail(ErrorMessages.Catalog.MeasuringUnitNotFound, ResultStatus.NotFound);
        }

        _unitOfWork.MeasuringUnits.Remove(unit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.Catalog.DeleteMeasuringUnit);
    }
}
