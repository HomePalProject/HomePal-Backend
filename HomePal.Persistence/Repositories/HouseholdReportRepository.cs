using System.Globalization;
using HomePal.Application.Features.Reports.DTOs;
using HomePal.Application.Features.Reports.Interfaces;
using HomePal.Domain.Common;
using HomePal.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HomePal.Persistence.Repositories;

public class HouseholdReportRepository : IHouseholdReportRepository
{
    private readonly ApplicationDbContext _context;

    public HouseholdReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HouseholdOverviewReportDto> GetHouseholdOverviewDataAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentMonth = now.Month;

        // 1. Pantry Items
        var pantryItems = await _context.PantryItems
            .AsNoTracking()
            .Include(pi => pi.Category)
            .Include(pi => pi.MeasuringUnit)
            .Where(pi => pi.Pantry.HouseholdId == householdId && !pi.IsDeleted)
            .ToListAsync(cancellationToken);

        // 2. Members count
        var totalMembersCount = await _context.HouseholdMembers
            .AsNoTracking()
            .CountAsync(m => m.HouseholdId == householdId && !m.IsDeleted, cancellationToken);

        // 3. Meal plans count
        var totalMealPlansCount = await _context.MealPlans
            .AsNoTracking()
            .CountAsync(mp => mp.HouseholdId == householdId && !mp.IsDeleted, cancellationToken);

        // 4. Monthly Budget for current period
        var currentBudgetDate = new DateTime(currentYear, currentMonth, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthlyBudget = await _context.HouseholdMonthlyBudgets
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.HouseholdId == householdId && b.BudgetDate == currentBudgetDate && !b.IsDeleted, cancellationToken);

        var budgetTarget = monthlyBudget?.Amount ?? 0m;

        // 5. Current Month Expenses
        var currentMonthExpenses = await _context.HouseholdExpenses
            .AsNoTracking()
            .Where(e => e.HouseholdId == householdId && e.ExpenseDate.Year == currentYear && e.ExpenseDate.Month == currentMonth && !e.IsDeleted)
            .SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;

        var monthlyRemaining = budgetTarget > 0 ? budgetTarget - currentMonthExpenses : 0m;

        var kpis = new HouseholdKpisDto
        {
            ItemsInInventory = pantryItems.Count,
            HouseholdMembers = totalMembersCount,
            MonthlyBudget = budgetTarget,
            MonthlyExpenses = currentMonthExpenses,
            MonthlyRemaining = monthlyRemaining,
            TotalGeneratedMealPlans = totalMealPlansCount
        };

        // 6. Expenses Over Time (Last 6 Months)
        var sixMonthsAgo = new DateTime(currentYear, currentMonth, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-5);
        var recentExpenses = await _context.HouseholdExpenses
            .AsNoTracking()
            .Where(e => e.HouseholdId == householdId && e.ExpenseDate >= sixMonthsAgo && !e.IsDeleted)
            .ToListAsync(cancellationToken);

        var expensesOverTime = new List<MonthlyExpenseTrendDto>();
        for (int i = 5; i >= 0; i--)
        {
            var targetDate = new DateTime(currentYear, currentMonth, 1).AddMonths(-i);
            var mYear = targetDate.Year;
            var mMonth = targetDate.Month;

            var sum = recentExpenses
                .Where(e => e.ExpenseDate.Year == mYear && e.ExpenseDate.Month == mMonth)
                .Sum(e => e.Amount);

            expensesOverTime.Add(new MonthlyExpenseTrendDto
            {
                Year = mYear,
                Month = mMonth,
                Amount = sum
            });
        }

        // 7. Most Bought Categories & Supermarkets
        var shoppingItems = await _context.ShoppingListItems
            .AsNoTracking()
            .Include(sli => sli.Category)
            .Include(sli => sli.Offer)
                .ThenInclude(o => o!.Supermarket)
            .Where(sli => sli.ShoppingList.HouseholdId == householdId && sli.IsPurchased && !sli.IsDeleted)
            .ToListAsync(cancellationToken);

        var topCategories = shoppingItems
            .Where(sli => sli.Category != null)
            .GroupBy(sli => sli.CategoryId)
            .Select(g => new CategoryPurchaseDto
            {
                CategoryName = g.First().Category!.Name ?? new List<LocalizedItem>(),
                PurchaseCount = g.Count()
            })
            .OrderByDescending(c => c.PurchaseCount)
            .Take(5)
            .ToList();

        // 8. Most Used Supermarkets
        var topSupermarkets = shoppingItems
            .Where(sli => sli.Offer?.Supermarket != null)
            .GroupBy(sli => sli.Offer!.SupermarketId)
            .Select(g => new SupermarketUsageDto
            {
                SupermarketName = g.First().Offer!.Supermarket!.Name ?? new List<LocalizedItem>(),
                PurchaseCount = g.Count()
            })
            .OrderByDescending(s => s.PurchaseCount)
            .Take(5)
            .ToList();

        // 9. Inventory Distribution by Category & Unit Breakdown
        var totalPantryCount = pantryItems.Count;
        var categoryDistribution = pantryItems
            .Where(pi => pi.Category != null)
            .GroupBy(pi => pi.CategoryId)
            .Select(g => new InventoryCategoryCountDto
            {
                CategoryName = g.First().Category!.Name ?? new List<LocalizedItem>(),
                Count = g.Count(),
                Percentage = totalPantryCount > 0 ? Math.Round((double)g.Count() / totalPantryCount * 100, 1) : 0
            })
            .OrderByDescending(c => c.Count)
            .ToList();

        var unitBreakdown = pantryItems
            .Where(pi => pi.MeasuringUnit != null)
            .GroupBy(pi => pi.MeasuringUnitId)
            .Select(g => new InventoryUnitBreakdownDto
            {
                UnitName = g.First().MeasuringUnit!.Name ?? new List<LocalizedItem>(),
                UnitSymbol = g.First().MeasuringUnit!.Symbol ?? new List<LocalizedItem>(),
                TotalQuantity = (double)Math.Round(g.Sum(pi => pi.Quantity), 2)
            })
            .OrderByDescending(u => u.TotalQuantity)
            .ToList();

        var inventoryDistribution = new InventoryDistributionDto
        {
            TotalItems = totalPantryCount,
            Categories = categoryDistribution,
            UnitBreakdown = unitBreakdown
        };

        // 10. Budget Overview
        var spentPercentage = budgetTarget > 0 ? Math.Round((double)(currentMonthExpenses / budgetTarget * 100), 1) : 0;
        var budgetOverview = new BudgetOverviewReportDto
        {
            Year = currentYear,
            Month = currentMonth,
            MonthlyTarget = budgetTarget,
            TotalSpent = currentMonthExpenses,
            Remaining = monthlyRemaining,
            SpentPercentage = spentPercentage
        };

        return new HouseholdOverviewReportDto
        {
            Kpis = kpis,
            ExpensesOverTime = expensesOverTime,
            MostBoughtCategories = topCategories,
            MostUsedSupermarkets = topSupermarkets,
            InventoryDistribution = inventoryDistribution,
            BudgetOverview = budgetOverview
        };
    }
}
