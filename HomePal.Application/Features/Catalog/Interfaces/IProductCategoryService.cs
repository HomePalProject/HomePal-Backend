using HomePal.Application.Features.Catalog.DTOs;
using HomePal.Shared.Results;
using Microsoft.AspNetCore.Http;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IProductCategoryService
{
    Task<Result<IReadOnlyCollection<ProductCategoryResponse>>> GetAllAsync(string? query = null, CancellationToken cancellationToken = default);
    Task<Result<ProductCategoryResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ProductCategoryResponse>> CreateAsync(CreateProductCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductCategoryResponse>> UpdateAsync(Guid id, UpdateProductCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductCategoryResponse>> UploadImageAsync(Guid id, IFormFile imageFile, CancellationToken cancellationToken = default);
    Task<Result<ProductCategoryResponse>> DeleteImageAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
