using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface ISupermarketRepository : IRepository<Supermarket>
{
    Task<IReadOnlyList<Supermarket>> SearchAsync(string? query, CancellationToken cancellationToken = default);
    Task<PaginatedList<Supermarket>> GetPagedAsync(
        PaginationRequest paginationRequest,
        string? query = null,
        CancellationToken cancellationToken = default);
}
