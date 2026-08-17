namespace HomePal.Application.Features.Reports.DTOs;

public class RevenueAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public string Currency { get; set; } = "EGP";
    public int ActiveSubscribers { get; set; }
    public int TotalTransactions { get; set; }
    public int SuccessfulTransactions { get; set; }
    public List<MonthlyRevenueTrendDto> MonthlyTrend { get; set; } = new();
}

public class MonthlyRevenueTrendDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int TransactionsCount { get; set; }
}
