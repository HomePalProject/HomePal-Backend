using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IOfferService
{
    Task<Result<PaginatedList<OfferResponse>>> GetPagedAsync(OfferQueryRequest request, CancellationToken cancellationToken = default);
    Task<Result<OfferResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<OfferResponse>> CreateAsync(CreateOfferRequest request, CancellationToken cancellationToken = default);
    Task<Result<OfferResponse>> UpdateAsync(Guid id, UpdateOfferRequest request, CancellationToken cancellationToken = default);
    Task<Result<OfferResponse>> UploadImageAsync(Guid id, IFormFile imageFile, CancellationToken cancellationToken = default);
    Task<Result<OfferResponse>> DeleteImageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
