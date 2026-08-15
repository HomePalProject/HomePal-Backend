using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Common.Interfaces;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByIdWithLocationAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetUserWithRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PaginatedList<ApplicationUser>> GetPagedUsersAsync(
        PaginationRequest paginationRequest,
        string? role = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}
