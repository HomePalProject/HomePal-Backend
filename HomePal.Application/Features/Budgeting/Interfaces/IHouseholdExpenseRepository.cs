using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Budgeting.Interfaces;

public interface IHouseholdExpenseRepository
{
    Task<HouseholdExpense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<HouseholdExpense>> GetByHouseholdAndPeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSpentByHouseholdAndPeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default);
    Task AddAsync(HouseholdExpense expense, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<HouseholdExpense> expenses, CancellationToken cancellationToken = default);
    void Remove(HouseholdExpense expense);
}
