using HomePal.Application.Common.Interfaces;
using HomePal.Domain.Entities;
using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.Catalog.Interfaces;

public interface IOfferRepository : IRepository<Offer>
{
    Task<Offer?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedList<Offer>> GetPagedAsync(
        PaginationRequest paginationRequest,
        string? query = null,
        Guid? categoryId = null,
        Guid? supermarketId = null,
        bool onlyVerified = true,
        CancellationToken cancellationToken = default);
}
