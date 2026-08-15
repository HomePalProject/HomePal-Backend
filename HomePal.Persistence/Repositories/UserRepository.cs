using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.Auth.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using HomePal.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ApplicationUser?> GetByIdWithLocationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Governorate)
            .Include(u => u.City)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Governorate)
            .Include(u => u.City)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Governorate)
            .Include(u => u.City)
            .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<ApplicationUser?> GetUserWithRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.RefreshTokens)
            .Include(u => u.Governorate)
            .Include(u => u.City)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<PaginatedList<ApplicationUser>> GetPagedUsersAsync(
        PaginationRequest paginationRequest,
        string? role = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var dbQuery = _dbSet.AsNoTracking()
            .Include(u => u.Governorate)
            .Include(u => u.City)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleName = role.Trim();
            dbQuery = from user in dbQuery
                      join userRole in _context.UserRoles on user.Id equals userRole.UserId
                      join r in _context.Roles on userRole.RoleId equals r.Id
                      where r.Name == roleName
                      select user;
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            dbQuery = dbQuery.Where(u =>
                (u.FullName != null && EF.Functions.Like(u.FullName, $"%{term}%")) ||
                (u.Email != null && EF.Functions.Like(u.Email, $"%{term}%")) ||
                (u.UserName != null && EF.Functions.Like(u.UserName, $"%{term}%")) ||
                (u.PhoneNumber != null && EF.Functions.Like(u.PhoneNumber, $"%{term}%")));
        }

        var count = await dbQuery.CountAsync(cancellationToken);

        var items = await dbQuery
            .OrderByDescending(u => u.CreatedAt)
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<ApplicationUser>.Create(items, count, paginationRequest.PageNumber, paginationRequest.PageSize);
    }
}
