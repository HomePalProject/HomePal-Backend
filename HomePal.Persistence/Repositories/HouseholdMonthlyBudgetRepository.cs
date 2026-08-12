using HomePal.Application.Features.Budgeting.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class HouseholdMonthlyBudgetRepository : Repository<HouseholdMonthlyBudget>, IHouseholdMonthlyBudgetRepository
{
    public HouseholdMonthlyBudgetRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<HouseholdMonthlyBudget?> GetByHouseholdAndPeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Expenses)
            .FirstOrDefaultAsync(b => b.HouseholdId == householdId && b.Year == year && b.Month == month, cancellationToken);
    }

    public async Task<HouseholdMonthlyBudget?> GetLatestBeforePeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(b => b.HouseholdId == householdId && (b.Year < year || (b.Year == year && b.Month < month)))
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
