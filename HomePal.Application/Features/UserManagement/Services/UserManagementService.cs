using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Auth.Options;
using HomePal.Application.Features.UserManagement.DTOs;
using HomePal.Application.Features.UserManagement.Interfaces;
using HomePal.Application.Features.UserManagement.Mappers;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HomePal.Application.Features.UserManagement.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AdminOptions _adminOptions;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IUnitOfWork unitOfWork,
        IOptions<AdminOptions> adminOptions)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _adminOptions = adminOptions.Value;
    }

    public async Task<Result<PaginatedList<UserResponse>>> GetUsersAsync(UserQueryRequest request, CancellationToken cancellationToken = default)
    {
        var pagedUsers = await _unitOfWork.Users.GetPagedUsersAsync(request, request.Role, request.SearchTerm, cancellationToken);

        var userResponses = new List<UserResponse>();
        foreach (var user in pagedUsers.Items)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userResponses.Add(user.ToResponse(roles));
        }

        var result = PaginatedList<UserResponse>.Create(
            userResponses,
            pagedUsers.TotalCount,
            pagedUsers.PageNumber,
            request.PageSize);

        return Result<PaginatedList<UserResponse>>.Ok(result, SuccessMessages.UserManagement.GetUsers);
    }

    public async Task<Result<UserResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<UserResponse>.Fail(ErrorMessages.UserManagement.UserNotFound, ResultStatus.NotFound);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<UserResponse>.Ok(user.ToResponse(roles), SuccessMessages.UserManagement.GetUser);
    }

    public async Task<Result<UserResponse>> AddAdminAsync(CreateAdminRequest request, CancellationToken cancellationToken = default)
    {
        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
        {
            return Result<UserResponse>.Fail(ErrorMessages.Auth.EmailExists, ResultStatus.Conflict);
        }

        var existingUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingUsername != null)
        {
            return Result<UserResponse>.Fail(ErrorMessages.Auth.UsernameExists, ResultStatus.Conflict);
        }

        var user = request.ToEntity();
        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => new Error(e.Description)).ToList();
            return Result<UserResponse>.Fail(ErrorMessages.UserManagement.AddAdminFailed, ResultStatus.BadRequest, errors);
        }

        if (!await _roleManager.RoleExistsAsync(Roles.Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Admin));
        }

        await _userManager.AddToRoleAsync(user, Roles.Admin);

        var roles = await _userManager.GetRolesAsync(user);
        return Result<UserResponse>.Ok(user.ToResponse(roles), SuccessMessages.UserManagement.AddAdmin, ResultStatus.Created);
    }

    public async Task<Result<UserResponse>> UpdateAdminAsync(Guid adminId, UpdateAdminRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(adminId.ToString());
        if (user == null)
        {
            return Result<UserResponse>.Fail(ErrorMessages.UserManagement.AdminNotFound, ResultStatus.NotFound);
        }

        if (!await _userManager.IsInRoleAsync(user, Roles.Admin))
        {
            return Result<UserResponse>.Fail(ErrorMessages.UserManagement.UserIsNotAdmin, ResultStatus.BadRequest);
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null && existingEmail.Id != user.Id)
            {
                return Result<UserResponse>.Fail(ErrorMessages.Auth.EmailExists, ResultStatus.Conflict);
            }
            user.Email = request.Email;
        }

        user.FullName = request.FullName;
        user.Gender = request.Gender;
        user.BirthDate = request.BirthDate;
        user.Governorate = request.Governorate;
        user.City = request.City;
        user.PhoneNumber = request.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = updateResult.Errors.Select(e => new Error(e.Description)).ToList();
            return Result<UserResponse>.Fail(ErrorMessages.UserManagement.UpdateAdminFailed, ResultStatus.BadRequest, errors);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Result<UserResponse>.Ok(user.ToResponse(roles), SuccessMessages.UserManagement.UpdateAdmin);
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Fail(ErrorMessages.UserManagement.UserNotFound, ResultStatus.NotFound);
        }

        if (IsProtectedAdmin(user))
        {
            return Result.Fail(ErrorMessages.UserManagement.CannotDeleteProtectedAdmin, ResultStatus.BadRequest);
        }

        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            var errors = deleteResult.Errors.Select(e => new Error(e.Description)).ToList();
            return Result.Fail(ErrorMessages.General, ResultStatus.BadRequest, errors);
        }

        return Result.Ok(SuccessMessages.UserManagement.DeleteUser);
    }

    public async Task<Result> DeactivateAccountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Fail(ErrorMessages.UserManagement.UserNotFound, ResultStatus.NotFound);
        }

        if (IsProtectedAdmin(user))
        {
            return Result.Fail(ErrorMessages.UserManagement.CannotDeactivateProtectedAdmin, ResultStatus.BadRequest);
        }

        user.IsActive = false;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = updateResult.Errors.Select(e => new Error(e.Description)).ToList();
            return Result.Fail(ErrorMessages.General, ResultStatus.BadRequest, errors);
        }

        return Result.Ok(SuccessMessages.UserManagement.DeactivateAccount);
    }

    public async Task<Result> DeleteAdminAsync(Guid adminId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(adminId.ToString());
        if (user == null)
        {
            return Result.Fail(ErrorMessages.UserManagement.AdminNotFound, ResultStatus.NotFound);
        }

        if (!await _userManager.IsInRoleAsync(user, Roles.Admin))
        {
            return Result.Fail(ErrorMessages.UserManagement.UserIsNotAdmin, ResultStatus.BadRequest);
        }

        if (IsProtectedAdmin(user))
        {
            return Result.Fail(ErrorMessages.UserManagement.CannotDeleteProtectedAdmin, ResultStatus.BadRequest);
        }

        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            var errors = deleteResult.Errors.Select(e => new Error(e.Description)).ToList();
            return Result.Fail(ErrorMessages.General, ResultStatus.BadRequest, errors);
        }

        return Result.Ok(SuccessMessages.UserManagement.DeleteAdmin);
    }

    private bool IsProtectedAdmin(ApplicationUser user)
    {
        if (string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(_adminOptions.Username) &&
            string.Equals(user.UserName, _adminOptions.Username, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
