using HomePal.Application.Features.RecipeManagament.Interfaces;
using HomePal.Domain.Entities.Recipe;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class RecipeRepository : Repository<Recipe> , IRecipeRepository
{
    public RecipeRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Recipe?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken);
    }

    public override async Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Steps)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Recipe?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToLower();

        return await _dbSet
            .FirstOrDefaultAsync(r =>
                r.Name.ToLower() == normalized,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Recipe>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllAsync(cancellationToken);

        var term = query.Trim().ToLower();

        return await _dbSet
            .AsNoTracking()
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Steps)
            .Where(r =>
                r.Name.ToLower().Contains(term) ||
                (r.Description != null &&
                 r.Description.ToLower().Contains(term)))
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }
}