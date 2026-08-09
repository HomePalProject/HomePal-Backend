using HomePal.Application.Features.Catalog.DTOs;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IScraperJobTracker
{
    ScraperJobStatusDto GetStatus();
    bool TryStartJob(Guid supermarketId);
    void UpdateProgress(int scrapedImagesIncrement, int extractedOffersIncrement);
    void CompleteJob();
    void FailJob(string errorMessage);
}
