namespace HomePal.Application.Features.Catalog.DTOs;

public class ScraperJobStatusDto
{
    public bool IsRunning { get; set; }
    public Guid? SupermarketId { get; set; }
    public int TotalScrapedImages { get; set; }
    public int TotalExtractedOffers { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string StatusMessage { get; set; } = "Idle";
    public string? ErrorMessage { get; set; }
}
