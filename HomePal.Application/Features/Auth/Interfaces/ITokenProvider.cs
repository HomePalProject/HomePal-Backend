using HomePal.Application.Features.Auth.DTOs;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Auth.Interfaces;

public interface ITokenProvider
{
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, IList<string> roles, CancellationToken cancellationToken = default);
    string GenerateRefreshToken();
}
