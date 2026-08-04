using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.Catalog.DTOs;

public class SupermarketQueryRequest : PaginationRequest
{
    public string? Query { get; set; }
}
