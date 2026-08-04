using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface ICanonicalProductRepository : IRepository<CanonicalProduct>
{
    Task<CanonicalProduct?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedList<CanonicalProduct>> GetPagedAsync(
        PaginationRequest paginationRequest,
        string? query = null,
        Guid? categoryId = null,
        CancellationToken cancellationToken = default);
}
