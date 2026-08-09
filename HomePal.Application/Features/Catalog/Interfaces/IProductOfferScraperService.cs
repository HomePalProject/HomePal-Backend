using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IProductOfferScraperService
{
    Task<Result<OfferScraperResultDto>> ProcessImageAsync(
        ProcessOfferImageRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<OfferScraperResultDto>> ScrapeFacebookPageAsync(
        ScrapeFacebookPageRequest request,
        CancellationToken cancellationToken = default);
}
