using HomePal.Shared.Results;
namespace HomePal.Application.Features.Auth.DTOs;

public class LoginResponse
{
    public CurrentUserResponse User { get; set; } = null!;
    public TokenResponse Tokens { get; set; } = null!;
}
