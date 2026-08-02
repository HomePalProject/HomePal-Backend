using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using HomePal.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
{
    public IngredientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PaginatedList<Ingredient>> GetPaginatedAsync(PaginationRequest request, string? search, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(i => i.Name.Any(n => n.Value.ToLower().Contains(searchLower)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(i => i.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<Ingredient>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var nameLower = name.Trim().ToLower();
        return await _dbSet.AnyAsync(i => i.Name.Any(n => n.Value.ToLower() == nameLower), cancellationToken);
    }

    public async Task<bool> IsUsedInRecipesAsync(Guid ingredientId, CancellationToken cancellationToken = default)
    {
        return await _context.RecipeIngredients.AnyAsync(ri => ri.IngredientId == ingredientId, cancellationToken);
    }
}
