using HomePal.Application.Features.Budgeting.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class HouseholdExpenseRepository : Repository<HouseholdExpense>, IHouseholdExpenseRepository
{
    public HouseholdExpenseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<HouseholdExpense>> GetByHouseholdAndPeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.HouseholdId == householdId && e.ExpenseDate.Year == year && e.ExpenseDate.Month == month)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalSpentByHouseholdAndPeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.HouseholdId == householdId && e.ExpenseDate.Year == year && e.ExpenseDate.Month == month)
            .SumAsync(e => e.Amount, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<HouseholdExpense> expenses, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(expenses, cancellationToken);
    }
}
