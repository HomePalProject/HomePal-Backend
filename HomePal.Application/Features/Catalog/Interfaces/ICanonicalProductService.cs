using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface ICanonicalProductService
{
    Task<Result<PaginatedList<CanonicalProductResponse>>> GetPagedAsync(CanonicalProductQueryRequest request, CancellationToken cancellationToken = default);
    Task<Result<CanonicalProductResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<CanonicalProductResponse>> CreateAsync(CreateCanonicalProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<CanonicalProductResponse>> UpdateAsync(Guid id, UpdateCanonicalProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<CanonicalProductResponse>> UploadImageAsync(Guid id, IFormFile imageFile, CancellationToken cancellationToken = default);
    Task<Result<CanonicalProductResponse>> DeleteImageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
