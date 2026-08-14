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
        var budgetDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        return await _dbSet
            .Include(b => b.Expenses)
            .FirstOrDefaultAsync(b => b.HouseholdId == householdId && b.BudgetDate == budgetDate, cancellationToken);
    }

    public async Task<HouseholdMonthlyBudget?> GetLatestBeforePeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default)
    {
        var budgetDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        return await _dbSet
            .Where(b => b.HouseholdId == householdId && b.BudgetDate < budgetDate)
            .OrderByDescending(b => b.BudgetDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
