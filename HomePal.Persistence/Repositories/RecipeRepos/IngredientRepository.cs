using HomePal.Application.Features.RecipeManagament.Interfaces;
using HomePal.Domain.Entities.Recipe;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class IngredientRepository : Repository<Ingredient>,IIngredientRepository
{
    public IngredientRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Recipes)
                .ThenInclude(ri => ri.Recipe)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Ingredient?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim().ToLower();

        return await _dbSet
            .FirstOrDefaultAsync(i =>
                i.Name.ToLower() == normalized,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Ingredient>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllAsync(cancellationToken);

        var term = query.Trim().ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(i => i.Name.ToLower().Contains(term))
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }
}