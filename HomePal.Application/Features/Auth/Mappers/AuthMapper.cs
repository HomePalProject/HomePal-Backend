using HomePal.Application.Features.Auth.DTOs;
using HomePal.Domain.Constants;
using HomePal.Domain.Entities;

namespace HomePal.Application.Features.Auth.Mappers;

public static class AuthMapper
{
    public static ApplicationUser ToEntity(this RegisterRequest request)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Gender = request.Gender,
            UserName = request.Username.Trim(),
            Email = request.Email.Trim(),
            BirthDate = request.BirthDate,
            Governorate = request.Governorate.Trim(),
            City = request.City.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static CurrentUserResponse ToCurrentUserResponse(this ApplicationUser user, IList<string> roles)
    {
        return new CurrentUserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Gender = user.Gender,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            BirthDate = user.BirthDate,
            Governorate = user.Governorate,
            City = user.City,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles
        };
    }

    public static UserProfileResponse ToUserProfileResponse(this ApplicationUser user, IList<string> roles)
    {
        return new UserProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Gender = user.Gender,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            BirthDate = user.BirthDate,
            Governorate = user.Governorate,
            City = user.City,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = roles
        };
    }
}
