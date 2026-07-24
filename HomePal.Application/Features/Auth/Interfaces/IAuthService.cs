using HomePal.Application.Features.Auth.DTOs;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.Auth.Interfaces;

public interface IAuthService
{
    Task<Result<CurrentUserResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<LoginResponse>> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(Guid userId, string? refreshToken = null, CancellationToken cancellationToken = default);
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request, CancellationToken cancellationToken = default);
    Task<Result<CurrentUserResponse>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
