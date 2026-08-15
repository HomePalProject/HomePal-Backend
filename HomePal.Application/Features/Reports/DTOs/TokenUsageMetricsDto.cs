using System.Text.Json.Serialization;

namespace HomePal.Application.Features.Reports.DTOs;

public class TokenUsageMetricsDto
{
    public long InputTokens { get; set; }
    public long OutputTokens { get; set; }
    public long TotalTokens { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime FromTimestamp { get; set; }
    public DateTime ToTimestamp { get; set; }
}

public class TokenMetricsFilterDto
{
    public DateTime? FromTimestamp { get; set; }
    public DateTime? ToTimestamp { get; set; }
}
