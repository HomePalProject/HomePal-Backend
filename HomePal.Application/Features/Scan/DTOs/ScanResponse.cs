namespace HomePal.Application.Features.Scan.DTOs;

public class ScanResponse
{
    public string RawText { get; set; } = string.Empty;
    public string DetectedName { get; set; } = string.Empty;
    public string DetectedCategory { get; set; } = string.Empty;
    public decimal SuggestedQuantity { get; set; }
    public string SuggestedUnit { get; set; } = string.Empty;
    public double Confidence { get; set; }
}
