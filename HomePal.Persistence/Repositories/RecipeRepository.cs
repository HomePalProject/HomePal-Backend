using HomePal.Application.Features.RecipeManagement.DTOs;
using HomePal.Application.Features.RecipeManagement.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using HomePal.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class RecipeRepository : Repository<Recipe>, IRecipeRepository
{
    public RecipeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PaginatedList<Recipe>> GetFilteredPaginatedAsync(RecipeFilterParams filter, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.MeasurementUnit)
            .Include(r => r.Preferences)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchLower = filter.Search.Trim().ToLower();
            query = query.Where(r => r.Name.Any(n => n.Value.ToLower().Contains(searchLower)) ||
                                     (r.Description != null && r.Description.Any(d => d.Value.ToLower().Contains(searchLower))));
        }

        if (filter.Difficulty.HasValue)
        {
            query = query.Where(r => r.Difficulty == filter.Difficulty.Value);
        }

        if (filter.MaxPrepTime.HasValue)
        {
            query = query.Where(r => r.PrepTimeMinutes == null || r.PrepTimeMinutes <= filter.MaxPrepTime.Value);
        }

        if (filter.MaxCookTime.HasValue)
        {
            query = query.Where(r => r.CookTimeMinutes == null || r.CookTimeMinutes <= filter.MaxCookTime.Value);
        }

        if (filter.MaxCalories.HasValue)
        {
            query = query.Where(r => r.CaloriesPerServing == null || r.CaloriesPerServing <= filter.MaxCalories.Value);
        }

        if (filter.PreferenceIds != null && filter.PreferenceIds.Count > 0)
        {
            query = query.Where(r => r.Preferences.Any(p => filter.PreferenceIds.Contains(p.Id)));
        }

        if (filter.IngredientIds != null && filter.IngredientIds.Count > 0)
        {
            query = query.Where(r => r.RecipeIngredients.Any(ri => filter.IngredientIds.Contains(ri.IngredientId)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return PaginatedList<Recipe>.Create(items, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<Recipe?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.MeasurementUnit)
            .Include(r => r.Preferences)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}
