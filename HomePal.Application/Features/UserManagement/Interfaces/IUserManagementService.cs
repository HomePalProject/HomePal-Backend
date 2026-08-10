using HomePal.Application.Features.UserManagement.DTOs;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.UserManagement.Interfaces;

public interface IUserManagementService
{
    Task<Result<PaginatedList<UserResponse>>> GetUsersAsync(UserQueryRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> AddAdminAsync(CreateAdminRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> UpdateAdminAsync(Guid adminId, UpdateAdminRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeactivateAccountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAdminAsync(Guid adminId, CancellationToken cancellationToken = default);
}
