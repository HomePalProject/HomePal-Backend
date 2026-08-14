using HomePal.Domain.Common;

namespace HomePal.Application.Features.Reports.DTOs;

public class HouseholdOverviewReportDto
{
    public HouseholdKpisDto Kpis { get; set; } = new();
    public List<MonthlyExpenseTrendDto> ExpensesOverTime { get; set; } = new();
    public List<CategoryPurchaseDto> MostBoughtCategories { get; set; } = new();
    public List<SupermarketUsageDto> MostUsedSupermarkets { get; set; } = new();
    public InventoryDistributionDto InventoryDistribution { get; set; } = new();
    public BudgetOverviewReportDto BudgetOverview { get; set; } = new();
}

public class HouseholdKpisDto
{
    public int ItemsInInventory { get; set; }
    public int HouseholdMembers { get; set; }
    public decimal MonthlyBudget { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal MonthlyRemaining { get; set; }
    public int TotalGeneratedMealPlans { get; set; }
}

public class MonthlyExpenseTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }
}

public class CategoryPurchaseDto
{
    public List<LocalizedItem> CategoryName { get; set; } = new();
    public int PurchaseCount { get; set; }
}

public class SupermarketUsageDto
{
    public List<LocalizedItem> SupermarketName { get; set; } = new();
    public int PurchaseCount { get; set; }
}

public class InventoryDistributionDto
{
    public int TotalItems { get; set; }
    public List<InventoryCategoryCountDto> Categories { get; set; } = new();
    public List<InventoryUnitBreakdownDto> UnitBreakdown { get; set; } = new();
}

public class InventoryCategoryCountDto
{
    public List<LocalizedItem> CategoryName { get; set; } = new();
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class InventoryUnitBreakdownDto
{
    public List<LocalizedItem> UnitName { get; set; } = new();
    public List<LocalizedItem> UnitSymbol { get; set; } = new();
    public double TotalQuantity { get; set; }
}

public class BudgetOverviewReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal MonthlyTarget { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal Remaining { get; set; }
    public double SpentPercentage { get; set; }
}
