using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Shared.Pagination;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface ISupermarketService
{
    Task<Result<IReadOnlyCollection<SupermarketResponse>>> GetAllAsync(string? query = null, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<SupermarketResponse>>> GetPagedAsync(SupermarketQueryRequest request, CancellationToken cancellationToken = default);
    Task<Result<SupermarketResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SupermarketResponse>> CreateAsync(CreateSupermarketRequest request, CancellationToken cancellationToken = default);
    Task<Result<SupermarketResponse>> UpdateAsync(Guid id, UpdateSupermarketRequest request, CancellationToken cancellationToken = default);
    Task<Result<SupermarketResponse>> UploadLogoAsync(Guid id, IFormFile imageFile, CancellationToken cancellationToken = default);
    Task<Result<SupermarketResponse>> DeleteLogoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
