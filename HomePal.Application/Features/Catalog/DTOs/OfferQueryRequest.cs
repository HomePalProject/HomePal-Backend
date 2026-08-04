using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.Catalog.DTOs;

public class OfferQueryRequest : PaginationRequest
{
    public string? Query { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SupermarketId { get; set; }
    public Guid? CanonicalProductId { get; set; }
    public bool? IsActiveNow { get; set; }
}
