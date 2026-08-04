using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.Catalog.DTOs;

public class CanonicalProductQueryRequest : PaginationRequest
{
    public string? Query { get; set; }
    public Guid? CategoryId { get; set; }
}
