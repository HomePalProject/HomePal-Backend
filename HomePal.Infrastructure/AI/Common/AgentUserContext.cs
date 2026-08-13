using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HomePal.Infrastructure.AI.Common;

/// <summary>
/// Resolves the authenticated user ID for AI agent tools.
/// </summary>
public class AgentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AgentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return Guid.Empty;

        var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? httpContext.User.FindFirstValue("sub");

        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
