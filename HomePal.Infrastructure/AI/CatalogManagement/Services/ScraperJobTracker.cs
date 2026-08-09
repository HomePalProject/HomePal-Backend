using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Application.Features.Catalog.Interfaces;

namespace HomePal.Infrastructure.AI.CatalogManagement.Services;

public class ScraperJobTracker : IScraperJobTracker
{
    private readonly object _lockObject = new();
    private readonly ScraperJobStatusDto _status = new();

    public ScraperJobStatusDto GetStatus()
    {
        lock (_lockObject)
        {
            return new ScraperJobStatusDto
            {
                IsRunning = _status.IsRunning,
                SupermarketId = _status.SupermarketId,
                TotalScrapedImages = _status.TotalScrapedImages,
                TotalExtractedOffers = _status.TotalExtractedOffers,
                StartedAt = _status.StartedAt,
                CompletedAt = _status.CompletedAt,
                StatusMessage = _status.StatusMessage,
                ErrorMessage = _status.ErrorMessage
            };
        }
    }

    public bool TryStartJob(Guid supermarketId)
    {
        lock (_lockObject)
        {
            if (_status.IsRunning)
                return false;

            _status.IsRunning = true;
            _status.SupermarketId = supermarketId;
            _status.TotalScrapedImages = 0;
            _status.TotalExtractedOffers = 0;
            _status.StartedAt = DateTime.UtcNow;
            _status.CompletedAt = null;
            _status.StatusMessage = "Scraping Facebook posts in background...";
            _status.ErrorMessage = null;

            return true;
        }
    }

    public void UpdateProgress(int scrapedImagesIncrement, int extractedOffersIncrement)
    {
        lock (_lockObject)
        {
            if (!_status.IsRunning)
                return;

            _status.TotalScrapedImages += scrapedImagesIncrement;
            _status.TotalExtractedOffers += extractedOffersIncrement;
            _status.StatusMessage = $"Processing image {_status.TotalScrapedImages}... ({_status.TotalExtractedOffers} offers extracted)";
        }
    }

    public void CompleteJob()
    {
        lock (_lockObject)
        {
            _status.IsRunning = false;
            _status.CompletedAt = DateTime.UtcNow;
            _status.StatusMessage = $"Completed successfully. Extracted {_status.TotalExtractedOffers} offers across {_status.TotalScrapedImages} images.";
        }
    }

    public void FailJob(string errorMessage)
    {
        lock (_lockObject)
        {
            _status.IsRunning = false;
            _status.CompletedAt = DateTime.UtcNow;
            _status.StatusMessage = "Scraping job failed.";
            _status.ErrorMessage = errorMessage;
        }
    }
}
