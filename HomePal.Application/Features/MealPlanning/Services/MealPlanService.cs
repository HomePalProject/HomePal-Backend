using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.MealPlanning.DTOs;
using HomePal.Application.Features.MealPlanning.Interfaces;
using HomePal.Application.Features.MealPlanning.Mappers;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.MealPlanning.Services;

public class MealPlanService : IMealPlanService
{
    private readonly IUnitOfWork _unitOfWork;

    public MealPlanService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private async Task<(HouseholdMember? Member, string? ErrorMessage, ResultStatus Status)> GetUserHouseholdMemberAsync(Guid userId, CancellationToken cancellationToken)
    {
        var member = await _unitOfWork.HouseholdMembers.GetByUserIdAsync(userId, cancellationToken);
        if (member == null)
        {
            return (null, ErrorMessages.MealPlan.NoHousehold, ResultStatus.NotFound);
        }

        return (member, null, ResultStatus.Success);
    }

    public async Task<Result<MealPlanResponse>> CreateMealPlanAsync(Guid userId, CreateMealPlanRequest request, CancellationToken cancellationToken = default)
    {
        var (member, errorMsg, status) = await GetUserHouseholdMemberAsync(userId, cancellationToken);
        if (errorMsg != null || member == null)
        {
            return Result<MealPlanResponse>.Fail(errorMsg ?? ErrorMessages.MealPlan.NoHousehold, status);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result<MealPlanResponse>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        if (request.EndDate < request.StartDate)
        {
            return Result<MealPlanResponse>.Fail(ErrorMessages.MealPlan.InvalidDates, ResultStatus.BadRequest);
        }

        var mealPlan = request.ToEntity(member.HouseholdId);
        await _unitOfWork.MealPlans.AddAsync(mealPlan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MealPlanResponse>.Ok(mealPlan.ToResponse(), SuccessMessages.MealPlan.Create, ResultStatus.Created);
    }

    public async Task<Result<MealPlanResponse>> GetMealPlanByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var (member, errorMsg, status) = await GetUserHouseholdMemberAsync(userId, cancellationToken);
        if (errorMsg != null || member == null)
        {
            return Result<MealPlanResponse>.Fail(errorMsg ?? ErrorMessages.MealPlan.NoHousehold, status);
        }

        var mealPlan = await _unitOfWork.MealPlans.GetByIdAndHouseholdIdAsync(id, member.HouseholdId, cancellationToken);
        if (mealPlan == null)
        {
            return Result<MealPlanResponse>.Fail(ErrorMessages.MealPlan.MealPlanNotFound, ResultStatus.NotFound);
        }

        return Result<MealPlanResponse>.Ok(mealPlan.ToResponse(), SuccessMessages.MealPlan.Get);
    }

    public async Task<Result<PaginatedList<MealPlanResponse>>> GetMealPlansAsync(Guid userId, PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
    {
        var (member, errorMsg, status) = await GetUserHouseholdMemberAsync(userId, cancellationToken);
        if (errorMsg != null || member == null)
        {
            return Result<PaginatedList<MealPlanResponse>>.Fail(errorMsg ?? ErrorMessages.MealPlan.NoHousehold, status);
        }

        var pagedMealPlans = await _unitOfWork.MealPlans.GetPagedByHouseholdIdAsync(member.HouseholdId, paginationRequest, cancellationToken);
        var dtos = pagedMealPlans.Items.Select(m => m.ToResponse()).ToList();

        var result = PaginatedList<MealPlanResponse>.Create(dtos, pagedMealPlans.TotalCount, pagedMealPlans.PageNumber, paginationRequest.PageSize);
        return Result<PaginatedList<MealPlanResponse>>.Ok(result, SuccessMessages.MealPlan.GetAll);
    }

    public async Task<Result<MealPlanResponse>> GetLastMealPlanAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var (member, errorMsg, status) = await GetUserHouseholdMemberAsync(userId, cancellationToken);
        if (errorMsg != null || member == null)
        {
            return Result<MealPlanResponse>.Fail(errorMsg ?? ErrorMessages.MealPlan.NoHousehold, status);
        }

        var mealPlan = await _unitOfWork.MealPlans.GetLastByHouseholdIdAsync(member.HouseholdId, cancellationToken);
        if (mealPlan == null)
        {
            return Result<MealPlanResponse>.Fail(ErrorMessages.MealPlan.MealPlanNotFound, ResultStatus.NotFound);
        }

        return Result<MealPlanResponse>.Ok(mealPlan.ToResponse(), SuccessMessages.MealPlan.GetLast);
    }

    public async Task<Result<MealPlanResponse>> UpdateMealPlanAsync(Guid userId, Guid id, UpdateMealPlanRequest request, CancellationToken cancellationToken = default)
    {
        var (member, errorMsg, status) = await GetUserHouseholdMemberAsync(userId, cancellationToken);
        if (errorMsg != null || member == null)
        {
            return Result<MealPlanResponse>.Fail(errorMsg ?? ErrorMessages.MealPlan.NoHousehold, status);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result<MealPlanResponse>.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        if (request.EndDate < request.StartDate)
        {
            return Result<MealPlanResponse>.Fail(ErrorMessages.MealPlan.InvalidDates, ResultStatus.BadRequest);
        }

        var mealPlan = await _unitOfWork.MealPlans.GetByIdAndHouseholdIdAsync(id, member.HouseholdId, cancellationToken);
        if (mealPlan == null)
        {
            return Result<MealPlanResponse>.Fail(ErrorMessages.MealPlan.MealPlanNotFound, ResultStatus.NotFound);
        }

        mealPlan.Title = request.Title.Trim();
        mealPlan.StartDate = request.StartDate;
        mealPlan.EndDate = request.EndDate;
        mealPlan.TotalEstimatedCost = request.TotalEstimatedCost;
        mealPlan.PlanData = request.PlanData;
        mealPlan.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.MealPlans.Update(mealPlan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MealPlanResponse>.Ok(mealPlan.ToResponse(), SuccessMessages.MealPlan.Update);
    }

    public async Task<Result> DeleteMealPlanAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var (member, errorMsg, status) = await GetUserHouseholdMemberAsync(userId, cancellationToken);
        if (errorMsg != null || member == null)
        {
            return Result.Fail(errorMsg ?? ErrorMessages.MealPlan.NoHousehold, status);
        }

        if (member.Role != Roles.HouseholdManager)
        {
            return Result.Fail(ErrorMessages.Household.NotManager, ResultStatus.Forbidden);
        }

        var mealPlan = await _unitOfWork.MealPlans.GetByIdAndHouseholdIdAsync(id, member.HouseholdId, cancellationToken);
        if (mealPlan == null)
        {
            return Result.Fail(ErrorMessages.MealPlan.MealPlanNotFound, ResultStatus.NotFound);
        }

        _unitOfWork.MealPlans.Remove(mealPlan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(SuccessMessages.MealPlan.Delete);
    }
}
