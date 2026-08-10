using HomePal.Shared.Pagination;

namespace HomePal.Application.Features.UserManagement.DTOs;

public class UserQueryRequest : PaginationRequest
{
    public string? Role { get; set; }
    public string? SearchTerm { get; set; }
}
