using HomePal.Application.Features.Catalog.DTOs;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IApifyScraperService
{
    Task<List<FacebookPostDto>> FetchFacebookPostsAsync(
        string pageUrl,
        int daysBack = 5,
        int resultsLimit = 20,
        CancellationToken cancellationToken = default);
}
