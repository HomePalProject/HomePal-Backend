using HomePal.Domain.Entities;

namespace HomePal.Application.Common.Interfaces;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetUserWithRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
