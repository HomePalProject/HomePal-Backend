using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Budgeting.Interfaces;

public interface IHouseholdMonthlyBudgetRepository
{
    Task<HouseholdMonthlyBudget?> GetByHouseholdAndPeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default);
    Task<HouseholdMonthlyBudget?> GetLatestBeforePeriodAsync(Guid householdId, int year, int month, CancellationToken cancellationToken = default);
    Task AddAsync(HouseholdMonthlyBudget budget, CancellationToken cancellationToken = default);
    void Update(HouseholdMonthlyBudget budget);
}
